# Fluxo do `GlobalExceptionHandler`

## Objetivo da classe

O `GlobalExceptionHandler` é o adaptador entre exceções .NET e respostas HTTP.
Ele recebe uma exceção que escapou da execução normal de um endpoint, decide
qual significado HTTP ela possui e escreve um `ProblemDetails` no response.

O código está em `Memo.API/Exceptions/GlobalExceptionHandler.cs`:

```csharp
internal sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
```

Há três detalhes importantes nessa declaração:

- `internal sealed`: a classe só é usada dentro do assembly da API e não foi
  feita para ser herdada;
- o `ILogger<GlobalExceptionHandler>` é recebido pelo primary constructor e
  fica disponível para registrar as falhas;
- `IExceptionHandler` é o contrato usado pelo middleware de exceções do
  ASP.NET Core.

A classe não é chamada pelos Controllers diretamente. O middleware
`UseExceptionHandler` chama `TryHandleAsync` quando uma exceção não tratada
acontece durante uma requisição.

## Como o handler entra no pipeline

Em `Program.cs`, o serviço e o middleware são registrados separadamente:

```csharp
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
```

`AddExceptionHandler<GlobalExceptionHandler>()` registra a implementação para
que o ASP.NET Core consiga construí-la via injeção de dependência. O logger do
primary constructor é resolvido automaticamente pelo container.

Depois que a aplicação é construída, o middleware é ativado:

```csharp
app.UseExceptionHandler();
```

Esse middleware fica antes de `MapControllers()`:

```csharp
app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
```

Assim, se um Controller, Validator, Handler, repositório ou qualquer outra
parte executada depois desse ponto lançar uma exceção, o middleware intercepta
essa exceção e chama o `GlobalExceptionHandler`.

O caminho de uma requisição com erro é:

```text
HTTP request
  -> Controller
  -> CustomMediator
  -> Validator ou Handler
  -> exceção
  -> ExceptionHandlerMiddleware
  -> GlobalExceptionHandler.TryHandleAsync
  -> HTTP response
```

Se nenhuma exceção for lançada, o handler não participa da resposta.

## Método `TryHandleAsync`

O método implementado pela classe é:

```csharp
public async ValueTask<bool> TryHandleAsync(
    HttpContext httpContext,
    Exception exception,
    CancellationToken cancellationToken)
```

Cada parâmetro tem uma função específica:

| Parâmetro | Uso no código |
|---|---|
| `httpContext` | lê o caminho da requisição e altera o status da resposta |
| `exception` | é classificada e transformada em `ProblemDetails` |
| `cancellationToken` | é usado ao escrever o JSON da resposta |

O retorno `ValueTask<bool>` informa ao middleware se a exceção foi tratada.
Neste projeto o método sempre retorna `true`, porque tanto exceções conhecidas
quanto desconhecidas recebem uma resposta.

## Primeira etapa: classificar a exceção

O primeiro bloco do método é:

```csharp
var problemDetails = exception switch
{
    ValidationException ex => CreateValidationProblem(ex),
    DomainException ex => Create(422, "Business Rule Violation", ex.Message),
    _ when IsUniqueViolation(exception) =>
        Create(409, "Conflict", "This record already exists."),
    _ when IsEntityNotFound(exception) =>
        Create(404, "Not Found", exception.Message),
    _ => null
};
```

Esse `switch` não executa handlers nem tenta corrigir o erro. Ele somente
produz uma das duas coisas:

- um `ProblemDetails`, se o tipo da exceção for reconhecido;
- `null`, se não houver uma regra específica para aquela exceção.

As linhas são avaliadas de cima para baixo. A ordem importa quando uma nova
exceção herda de outra ou quando mais de uma regra poderia reconhecer o mesmo
objeto. A regra mais específica deve aparecer antes da regra genérica.

### Padrão `ValidationException ex`

```csharp
ValidationException ex => CreateValidationProblem(ex)
```

O padrão verifica se `exception` é uma `ValidationException`. Quando combina,
ele também cria a variável local `ex` com o tipo correto, permitindo que o
método leia `exception.Errors`.

Essa exceção é criada pelo `CustomMediator`:

```csharp
var validationResult = await validator.ValidateAsync(typedRequest, ct);
if (!validationResult.IsValid)
    throw new ValidationException(validationResult.Errors);
```

Portanto, o Validator não cria uma resposta HTTP. Ele apenas produz os erros,
o Mediator lança a exceção e o `GlobalExceptionHandler` converte o resultado
em HTTP `400`.

### Padrão `DomainException ex`

```csharp
DomainException ex =>
    Create(422, "Business Rule Violation", ex.Message)
```

`DomainException` é uma exceção concreta do Domain. O handler usa a mensagem
da exceção como `Detail`, porque essa mensagem descreve a regra que foi
violada, por exemplo:

```csharp
throw new DomainException("Commission must be between 0% and 100%.");
```

O resultado será um `ProblemDetails` com status `422`, título
`Business Rule Violation` e o mesmo texto no campo `detail`.

### Guard clause `_ when IsUniqueViolation(exception)`

```csharp
_ when IsUniqueViolation(exception) =>
    Create(409, "Conflict", "This record already exists.")
```

`_` significa que não é necessário capturar uma variável por tipo. A condição
real está no `when`: o método `IsUniqueViolation` precisa retornar `true`.

Essa regra aparece depois de `DomainException` porque não depende apenas do
tipo externo da exceção. O Entity Framework encapsula o erro do PostgreSQL em
uma `DbUpdateException`, e o PostgreSQL fica no `InnerException`.

### Guard clause `_ when IsEntityNotFound(exception)`

```csharp
_ when IsEntityNotFound(exception) =>
    Create(404, "Not Found", exception.Message)
```

Aqui também há uma condição adicional. O tipo não é testado com
`exception is EntityNotFoundException<object>`, porque
`EntityNotFoundException<TEntity>` é genérico e cada entidade produz um tipo
fechado diferente.

O helper verifica a definição genérica e aceita, por exemplo:

```text
EntityNotFoundException<Customer>
EntityNotFoundException<Company>
EntityNotFoundException<Seller>
```

### Caso `_ => null`

Qualquer exceção que não combine com as regras anteriores chega ao último caso:

```csharp
_ => null
```

O `null` não é enviado ao cliente. Ele sinaliza que a exceção é inesperada e
faz o código seguinte criar uma resposta `500`.

## Segunda etapa: logging e fallback

Depois do `switch`, o código verifica o resultado:

```csharp
if (problemDetails is null)
{
    logger.LogError(exception, "Unhandled exception");
    problemDetails = Create(
        StatusCodes.Status500InternalServerError,
        "Internal Server Error",
        "An unexpected error occurred. Please try again later.");
}
else
{
    logger.LogWarning(
        "Request failed ({Status}): {Message}",
        problemDetails.Status,
        exception.Message);
}
```

### Exceção desconhecida

Quando o `ProblemDetails` é `null`, a exceção é inesperada:

```csharp
logger.LogError(exception, "Unhandled exception");
```

O objeto `exception` é passado como primeiro argumento estruturado do logger.
Isso preserva stack trace e causa interna no log, sem enviar esses dados ao
cliente.

Em seguida, o método usa `Create` para construir o fallback `500`. A mensagem
é fixa de propósito:

```text
An unexpected error occurred. Please try again later.
```

A mensagem original pode conter SQL, nomes de tabelas, caminhos de arquivos
ou detalhes de infraestrutura, por isso não é usada como `detail` da
resposta.

### Exceção reconhecida

Se o `switch` criou um problema, o código registra:

```csharp
logger.LogWarning(
    "Request failed ({Status}): {Message}",
    problemDetails.Status,
    exception.Message);
```

Falhas esperadas de validação, domínio, busca e unicidade são `Warning`, não
`Error`. O status escolhido e a mensagem original são incluídos nos campos
estruturados do log.

O handler não retorna nesse ponto. Depois de registrar, ele ainda precisa
preencher o contexto HTTP e escrever o JSON.

## Terceira etapa: preencher o `ProblemDetails`

Todo caminho de execução chega a estas linhas:

```csharp
problemDetails.Instance = httpContext.Request.Path;
httpContext.Response.StatusCode = problemDetails.Status!.Value;
await httpContext.Response.WriteAsJsonAsync(
    problemDetails,
    cancellationToken);

return true;
```

### `Instance`

```csharp
problemDetails.Instance = httpContext.Request.Path;
```

`Request.Path` é o caminho acessado, como `/api/customers` ou
`/api/sellers/{id}`. Ele é copiado para `ProblemDetails.Instance` para que o
cliente e os logs de integração saibam em qual recurso a falha ocorreu.

### `Status`

```csharp
httpContext.Response.StatusCode = problemDetails.Status!.Value;
```

`ProblemDetails.Status` é declarado como `int?`, mas todos os objetos criados
por este handler recebem status em `Create`. O operador `!` informa ao
compilador que o valor não é nulo nesse ponto, e `.Value` extrai o `int`
necessário pelo `HttpResponse`.

### Escrita do JSON

```csharp
await httpContext.Response.WriteAsJsonAsync(
    problemDetails,
    cancellationToken);
```

O ASP.NET Core serializa o objeto para o corpo da resposta. O token recebido
do pipeline é encaminhado para que uma requisição cancelada não continue
escrevendo a resposta.

Por fim:

```csharp
return true;
```

O `true` informa que a exceção foi tratada. Se o método retornasse `false`, o
middleware poderia continuar procurando outro handler ou aplicar o tratamento
padrão.

## Helper `Create`

O método usado pelos erros simples é:

```csharp
private static ProblemDetails Create(
    int status,
    string title,
    string detail) =>
    new()
    {
        Status = status,
        Title = title,
        Detail = detail
    };
```

Ele centraliza a criação dos três campos principais e evita repetir `new
ProblemDetails` em cada ramo do `switch`.

O método é `static` porque não usa `logger`, `HttpContext` nem qualquer estado
da instância. Ele recebe tudo o que precisa nos argumentos.

O `Instance` não é preenchido nesse helper porque depende da requisição atual.
Ele é adicionado posteriormente, em um único ponto comum a todas as exceções.

## Helper `CreateValidationProblem`

Validação é diferente dos outros erros porque uma requisição pode conter
várias falhas e várias falhas podem pertencer à mesma propriedade:

```csharp
private static ProblemDetails CreateValidationProblem(
    ValidationException exception)
{
    var errors = exception.Errors
        .GroupBy(e => e.PropertyName)
        .ToDictionary(
            g => g.Key,
            g => g.Select(e => e.ErrorMessage).ToArray());
    var problemDetails = Create(
        StatusCodes.Status400BadRequest,
        "Request Failed",
        "One or more validation errors occurred.");
    problemDetails.Extensions["errors"] = errors;
    return problemDetails;
}
```

O processamento acontece em etapas:

1. `exception.Errors` fornece todos os `ValidationFailure` do FluentValidation;
2. `GroupBy(e => e.PropertyName)` cria um grupo por campo;
3. `g.Key` vira a chave do dicionário, como `Name` ou `PageSize`;
4. `Select(e => e.ErrorMessage)` extrai somente as mensagens;
5. `ToArray()` cria o array de mensagens daquela propriedade;
6. `Create(...)` cria o problema base com status `400`;
7. `Extensions["errors"]` adiciona o dicionário sem criar uma classe de
   resposta específica.

Para duas falhas no mesmo campo:

```text
Name -> ["Name is required.", "Name is too short."]
```

O corpo resultante mantém as duas mensagens:

```json
{
  "status": 400,
  "title": "Request Failed",
  "detail": "One or more validation errors occurred.",
  "instance": "/api/customers",
  "errors": {
    "Name": [
      "Name is required.",
      "Name is too short."
    ]
  }
}
```

`Extensions` pertence ao próprio modelo `ProblemDetails` e é o mecanismo
apropriado para campos adicionais que não fazem parte do conjunto básico
`status`, `title`, `detail` e `instance`.

## Helper `IsUniqueViolation`

O método é uma expression-bodied method:

```csharp
static bool IsUniqueViolation(Exception ex) =>
    ex is DbUpdateException
    {
        InnerException: PostgresException
        {
            SqlState: PostgresErrorCodes.UniqueViolation
        }
    };
```

Esse pattern matching verifica tudo em uma expressão:

1. o objeto precisa ser uma `DbUpdateException`;
2. sua `InnerException` precisa ser uma `PostgresException`;
3. o `SqlState` dessa exceção precisa ser o código PostgreSQL de violação de
   unicidade.

A checagem não usa apenas `ex is DbUpdateException`, porque essa exceção
também pode representar falha de conexão, constraint diferente ou outro erro
de persistência. Somente a combinação específica é convertida em `409`.

O cliente recebe uma mensagem genérica:

```csharp
Create(409, "Conflict", "This record already exists.")
```

O nome da constraint e a mensagem do PostgreSQL permanecem somente no erro
interno/logs.

## Helper `IsEntityNotFound`

O método é:

```csharp
private static bool IsEntityNotFound(Exception exception) =>
    exception.GetType() is { IsGenericType: true } type &&
    type.GetGenericTypeDefinition() == typeof(EntityNotFoundException<>);
```

A expressão usa `&&` com avaliação de curto-circuito:

- primeiro, `GetType()` precisa ser um tipo genérico;
- somente nesse caso a variável `type` é usada;
- depois, `GetGenericTypeDefinition()` remove o argumento concreto e retorna
  a definição aberta `EntityNotFoundException<>`;
- a comparação confirma que a exceção pertence exatamente a essa família.

Isso reconhece `EntityNotFoundException<Customer>`, por exemplo, sem precisar
adicionar um `case` separado para cada entidade.

A mensagem é preservada:

```csharp
Create(404, "Not Found", exception.Message)
```

O construtor da exceção já produz mensagens como `Customer not found.` usando
`typeof(TEntity).Name`.

## Mapa completo de respostas

| Exceção reconhecida | Status | `title` | Origem |
|---|---:|---|---|
| `ValidationException` | 400 | `Request Failed` | `CustomMediator` após FluentValidation |
| `DomainException` | 422 | `Business Rule Violation` | Entidades e value objects |
| `DbUpdateException` + `UniqueViolation` | 409 | `Conflict` | PostgreSQL/EF Core |
| `EntityNotFoundException<TEntity>` | 404 | `Not Found` | Handlers de consulta/alteração |
| qualquer outra | 500 | `Internal Server Error` | fallback do handler |

## Exemplo completo de execução

Considere uma Query inválida:

```http
GET /api/customers?page=0&pageSize=10
```

1. `CustomerController.GetAll` cria `GetAllCustomersQuery(0, 10)`;
2. `CustomMediator` procura `IValidator<GetAllCustomersQuery>`;
3. o Validator falha na regra `Page.GreaterThan(0)`;
4. o Mediator executa `throw new ValidationException(...)`;
5. a exceção sobe pelo Controller até `UseExceptionHandler`;
6. o middleware chama `TryHandleAsync`;
7. `exception switch` combina com `ValidationException ex`;
8. `CreateValidationProblem` agrupa a falha em `errors`;
9. `LogWarning` registra o status `400`;
10. `Instance` recebe `/api/customers`;
11. `Response.StatusCode` recebe `400`;
12. `WriteAsJsonAsync` escreve o JSON;
13. `return true` encerra o tratamento.

Para uma exceção não reconhecida, os passos 7 e 8 mudam: o `switch` retorna
`null`, `LogError` registra stack trace, o fallback cria `500` e o detalhe
interno é substituído pela mensagem genérica.

## Testes e relação com o código

`Memo.API.Tests/Exceptions/GlobalExceptionHandlerTests.cs` instancia o handler
com `NullLogger<GlobalExceptionHandler>` e chama `TryHandleAsync` diretamente.
O teste cria um `DefaultHttpContext`, define o `Request.Path`, usa um
`MemoryStream` como corpo da resposta e depois verifica:

- o `StatusCode`;
- o `title` serializado;
- o `detail` ou a extensão `errors`.

Essa abordagem testa a própria classificação e a escrita da resposta sem
precisar iniciar um servidor HTTP. Ela cobre os quatro comportamentos
principais: `400`, `422`, `404` e `500`.

Ao acrescentar uma nova regra ao `switch`, o teste correspondente deve validar
o padrão de matching, o status HTTP e o conteúdo produzido pelo helper usado.
