# Arquitetura do Memo.API

## Visão geral

O Memo.API utiliza **Clean Architecture**, organizadando em camadas e casos de uso, 
incorporando princípios de **Domain-Driven Design (DDD)** e o padrão **CQRS (Command Query 
Responsibility Segregation)**.

A arquitetura promove a separação de responsabilidades e o isolamento das regras de negócio 
em relação aos detalhes técnicos da aplicação. 
Para isso, o projeto é dividido em quatro camadas principais:

- **Domain:** núcleo da aplicação, responsável pelas entidades, regras de negócio, 
invariantes e contratos de abstração, como repositories e UnitOfWork.

- **Application:** responsável pela orquestração dos casos de uso, contendo Commands, 
Queries, Handlers, validações e modelos de resposta.

- **Infrastructure:** responsável pelas implementações técnicas, incluindo persistência,
Entity Framework Core, repositories, UnitOfWork e serviços de infraestrutura.

- **API:** camada de entrada e comunicação HTTP, responsável pelos controllers, 
configuração da aplicação, middlewares e tratamento global de exceções.

### Diagrama da arquitetura

```mermaid
flowchart TB
    Client[Cliente HTTP]

    subgraph API["Memo.API - transporte HTTP"]
        Controller[Controllers]
        ExceptionHandler[GlobalExceptionHandler]
    end

    subgraph Application["Memo.Application - casos de uso"]
        Mediator[CustomMediator]
        Validator[FluentValidation]
        Handler[Command / Query Handlers]
    end

    subgraph Domain["Memo.Domain - núcleo"]
        Entity[Entidades e regras de negócio]
        Contracts[Contratos de repositories e UnitOfWork]
        Common[Auditoria, exceções e paginação]
    end

    subgraph Infra["Memo.Infra - implementações técnicas"]
        Repository[Repositories]
        UnitOfWork[UnitOfWork]
        DbContext[AppDbContext]
        EF[Entity Framework Core]
    end

    Database[(PostgreSQL)]

    Client --> Controller
    Controller --> Mediator
    Mediator --> Validator
    Validator --> Handler
    Handler --> Contracts
    Contracts -. implementados por .-> Repository
    Contracts -. implementados por .-> UnitOfWork
    Repository --> DbContext
    UnitOfWork --> DbContext
    DbContext --> EF
    EF --> Database
    Controller -. exceções .-> ExceptionHandler
    Handler --> Entity
    Entity --> Common
```

O fluxo principal percorre a API, a Application e as abstrações do Domain até
as implementações da Infra e o PostgreSQL. As setas pontilhadas representam
inversão de dependência: os contratos são definidos no Domain, mas suas
implementações ficam na Infra.

### Regra de dependência

As dependências apontam para dentro: a **API** usa a **Application**, a **Application**
usa abstrações definidas no **Domain**, e a **Infra** implementa essas abstrações.
O Domain não depende das camadas externas. Com isso, as regras de negócio ficam independentes 
do banco de dados e do framework HTTP, os casos de uso podem ser testados de forma isolada e 
detalhes técnicos podem ser trocados sem alterar o núcleo.

### CQRS

O CQRS organiza as operações da aplicação em dois tipos de requisição: Commands, responsáveis por 
alterar o estado do sistema, e Queries, responsáveis exclusivamente pela consulta de informações. 
Um Mediator próprio (`CustomMediator`) realiza o encaminhamento dessas requisições para seus respectivos Handlers, 
centralizando o fluxo de execução e a validação dos requests.

### DDD

Os princípios de DDD são aplicados principalmente na modelagem do domínio, 
por meio de entidades que encapsulam regras de negócio, protegem suas invariantes e 
controlam as alterações de estado. O projeto também utiliza abstrações de repositórios 
para separar o domínio dos mecanismos de persistência.

O projeto utiliza:

- **.NET 10** e ASP.NET Core;
- **PostgreSQL** com Entity Framework Core;
- **CQRS** para separar comandos e consultas;
- um **Mediator próprio** para executar handlers;
- **FluentValidation** para validar requests;
- entidades auditáveis com **soft-delete**;
- paginação reutilizável;
- `CancellationToken` propagado da requisição HTTP até o EF Core;
- testes separados por camada.

## Estrutura da solução

```text
Memo.API.sln
├── Memo.API
├── Memo.Application
├── Memo.Domain
├── Memo.Infra
```

### Memo.Domain

É o núcleo do sistema. Contém regras e abstrações que não dependem de
ASP.NET Core, Entity Framework ou banco de dados.

Responsabilidades:

- entidades e regras de negócio;
- exceções de domínio;
- contratos de repositories;
- contratos de `IUnitOfWork` e contexto do usuário;
- classes comuns, como auditoria e paginação.

Principais diretórios:

```text
Memo.Domain/
├── Common/
├── Entities/
├── Exceptions/
└── IRepositories/
```

Uma entidade persistida segue o padrão de `AuditableEntity`. Isso fornece:

- `Id`;
- `CreatedAt` e `CreatedBy`;
- `UpdatedAt` e `UpdatedBy`;
- `IsDeleted`;
- `DeletedAt` e `DeletedBy`.

As propriedades têm setters protegidos, e as alterações devem ser feitas por
métodos da própria entidade, como `Update`.

### Memo.Application

Implementa os casos de uso da aplicação. Essa camada coordena validação,
regras de fluxo, repositories e persistência, mas não conhece detalhes do
PostgreSQL.

Cada feature é organizada por entidade e operação:

```text
Features/
└── Companies/
    ├── Commands/
    │   ├── Create/
    │   ├── Update/
    │   └── Delete/
    └── Queries/
        ├── GetAll/
        ├── GetById/
        └── Response/
```

Cada operação normalmente possui:

- um `record` que representa o request;
- um `Validator`;
- um `Handler`;
- uma resposta específica, quando necessário.

Comandos alteram estado (`Create`, `Update`, `Delete`). Queries apenas
consultam dados (`GetAll`, `GetById`).

### Memo.Infra

Implementa os detalhes técnicos da aplicação:

- `AppDbContext`;
- configurações do Entity Framework;
- repositories concretos;
- `UnitOfWork`;
- paginação;
- migrations;
- contexto do usuário.

Principais diretórios:

```text
Memo.Infra/
├── Migrations/
├── Persistence/
│   └── Configuration/
├── Repositories/
└── Services/
```

O `AppDbContext` aplica automaticamente as configurações encontradas no
assembly e cria filtros globais para não retornar entidades com
`IsDeleted = true`.

Exclusões são convertidas em alterações durante `SaveChangesAsync`: o registro
não é removido fisicamente; ele recebe os dados de soft-delete.

### Memo.API

É a camada de transporte HTTP. Contém:

- controllers;
- configuração da aplicação;
- tratamento global de exceções;
- Swagger/OpenAPI;
- configuração de middleware.

Controllers não devem conter regras de negócio. Eles recebem a requisição,
montam ou recebem o command/query, encaminham para o mediator e transformam o
resultado em resposta HTTP.

## Fluxo de uma requisição

O fluxo normal é:

```text
HTTP request
    ↓
Controller
    ↓
IMediator.SendAsync(request, ct)
    ↓
Validator
    ↓
IRequestHandler.HandleAsync(request, ct)
    ↓
Repository / UnitOfWork
    ↓
Entity Framework Core
    ↓
PostgreSQL
```

O `CustomMediator` localiza o validator e o handler pelo tipo do request.
Quando a validação falha, ele lança `FluentValidation.ValidationException`.
Quando não existe handler registrado, lança uma exceção explícita.

O `GlobalExceptionHandler` converte as exceções conhecidas em respostas
`ProblemDetails`, incluindo erros de validação, regras de domínio, entidade
não encontrada e violação de unicidade.

## CancellationToken

O controller é a entrada do token associado à requisição HTTP. Os endpoints
podem declarar `CancellationToken ct = default` e devem passá-lo ao mediator.

Nas camadas internas, o token é obrigatório:

```text
Controller: ct = default
Mediator: ct
Handler: ct
Repository: ct
Pagination service: ct
UnitOfWork: ct
EF Core: ct
```

Operações assíncronas que acessam o banco devem usar o token:

```csharp
await query.CountAsync(ct);
await query.ToListAsync(ct);
await context.SaveChangesAsync(ct);
```

Chamadas síncronas como `Add`, `Update` e `Delete` apenas alteram o
`DbContext` em memória. A operação de banco ocorre no `SaveChangesAsync(ct)`.

## Padrão dos endpoints

Os controllers seguem convenções REST:

| Operação | Rota | Resposta esperada |
|---|---|---|
| Listar | `GET /api/companies` | `200 OK` |
| Buscar por ID | `GET /api/companies/{id}` | `200 OK` ou `404 Not Found` |
| Criar | `POST /api/companies` | `201 Created` |
| Atualizar | `PUT /api/companies/{id}` | `204 No Content` |
| Excluir logicamente | `DELETE /api/companies/{id}` | `204 No Content` |

O mesmo padrão é usado para `Seller`.

O `POST` retorna `CreatedAtAction`, apontando para o endpoint de busca por ID.
No `PUT`, o ID da rota prevalece sobre qualquer ID enviado no body:

```csharp
await mediator.SendAsync(command with { Id = id }, ct);
```

## Validação, auditoria e unicidade

A validação ocorre em dois níveis:

1. **FluentValidation**, para validar a entrada da aplicação;
2. **domínio**, para preservar invariantes mesmo quando a entidade é criada
   fora da API.

A entidade `Company`, por exemplo, garante que:

- o nome não seja vazio;
- a comissão esteja entre `0` e `100`.

Índices únicos de entidades com soft-delete devem considerar somente registros
ativos. Por exemplo:

```csharp
builder.HasIndex(company => company.Name)
    .IsUnique()
    .HasFilter("\"IsDeleted\" = false");
```

Assim, nomes podem ser reutilizados depois de uma exclusão lógica, mas
continuam únicos entre registros ativos.

## Como criar uma nova entidade

O exemplo abaixo usa uma entidade hipotética chamada `Product`. Adapte os
nomes e regras ao domínio real.

### 1. Criar a entidade no Domain

Crie `Memo.Domain/Entities/Product.cs`.

```csharp
using Memo.Domain.Common;
using Memo.Domain.Exceptions;

namespace Memo.Domain.Entities;

public sealed class Product : AuditableEntity
{
    public string Name { get; private set; } = null!;
    public decimal Price { get; private set; }

    private Product()
    {
    }

    public Product(string name, decimal price)
    {
        Validate(name, price);
        Name = name;
        Price = price;
    }

    public void Update(string name, decimal price)
    {
        Validate(name, price);
        Name = name;
        Price = price;
    }

    private static void Validate(string name, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name cannot be null or empty.");

        if (price < 0)
            throw new DomainException("Price cannot be negative.");
    }
}
```

O construtor sem parâmetros deve permanecer privado para o EF Core. A
inicialização `= null!` evita o warning de nulabilidade causado por esse
construtor.

### 2. Criar o contrato do repository

Crie `Memo.Domain/IRepositories/IProductRepository.cs`.

```csharp
using Memo.Domain.Common;
using Memo.Domain.Entities;

namespace Memo.Domain.IRepositories;

public interface IProductRepository
{
    Task<PagedList<Product>> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken ct);

    Task<Product?> GetByIdAsync(
        Guid id,
        CancellationToken ct);

    Product Add(Product product);
    Product Update(Product product);
    Product Delete(Product product);
}
```

Repositories devem exigir `CancellationToken`; apenas os controllers são a
fronteira que pode fornecer `= default`.

### 3. Adicionar a configuração do EF Core

Crie `Memo.Infra/Persistence/Configuration/ProductConfiguration.cs`.

```csharp
using Memo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Memo.Infra.Persistence.Configuration;

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(product => product.Id);
        builder.Property(product => product.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(product => product.Price)
            .IsRequired()
            .HasPrecision(12, 2);

        builder.HasIndex(product => product.Name)
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");
    }
}
```

Como o `AppDbContext` usa `ApplyConfigurationsFromAssembly`, a configuração
será descoberta automaticamente.

### 4. Registrar o DbSet

Em `Memo.Infra/Persistence/AppDbContext.cs`, adicione:

```csharp
public DbSet<Product> Products => Set<Product>();
```

O filtro global de soft-delete e o processamento de auditoria já são aplicados
automaticamente por `AppDbContext`, pois `Product` herda de `AuditableEntity`.

### 5. Implementar o repository

Crie `Memo.Infra/Repositories/ProductRepository.cs`.

```csharp
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.IRepositories;
using Memo.Infra.Persistence;
using Memo.Infra.Services;
using Microsoft.EntityFrameworkCore;

namespace Memo.Infra.Repositories;

internal sealed class ProductRepository(
    AppDbContext context,
    IPaginationService pagination) : IProductRepository
{
    public async Task<PagedList<Product>> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken ct)
    {
        IQueryable<Product> query = context.Products
            .AsNoTracking()
            .OrderBy(product => product.CreatedAt)
            .ThenBy(product => product.Id);

        return await pagination.PaginateAsync(
            query,
            page,
            pageSize,
            ct);
    }

    public Task<Product?> GetByIdAsync(
        Guid id,
        CancellationToken ct)
    {
        return context.Products
            .FirstOrDefaultAsync(product => product.Id == id, ct);
    }

    public Product Add(Product product)
    {
        context.Products.Add(product);
        return product;
    }

    public Product Update(Product product)
    {
        context.Products.Update(product);
        return product;
    }

    public Product Delete(Product product)
    {
        context.Products.Remove(product);
        return product;
    }
}
```

### 6. Registrar a implementação no DI

Em `Memo.Infra/DependencyInjection.cs`:

```csharp
services.AddScoped<IProductRepository, ProductRepository>();
```

Sem esse registro, o mediator encontrará o handler, mas a injeção do
repository falhará em runtime.

### 7. Criar commands, queries e handlers

Crie uma pasta `Memo.Application/Features/Products` com a mesma organização
usada por `Companies`:

```text
Products/
├── Commands/
│   ├── Create/
│   ├── Update/
│   └── Delete/
└── Queries/
    ├── GetAll/
    ├── GetById/
    └── Response/
```

Cada request deve implementar `IRequest<TResponse>`. Cada handler deve:

- receber o repository por injeção;
- receber `CancellationToken ct` sem valor padrão;
- passar `ct` para todas as operações assíncronas;
- lançar `EntityNotFoundException<Product>` quando necessário;
- chamar `SaveChangesAsync(ct)` em comandos que alteram dados.

Não é necessário registrar handlers manualmente. O `AddApplication` faz
scan do assembly e registra automaticamente implementações de
`IRequestHandler<,>`.

### 8. Criar validators

Cada command e query que precisar de validação deve ter um validator interno
derivado de `AbstractValidator<T>`.

Exemplo:

```csharp
internal sealed class CreateProductValidator
    : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(product => product.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(product => product.Price)
            .GreaterThanOrEqualTo(0);
    }
}
```

Os validators são descobertos automaticamente por
`AddValidatorsFromAssembly(..., includeInternalTypes: true)`.

### 9. Criar o response

Não exponha diretamente a entidade do domínio pela API. Crie um response
específico, por exemplo:

```csharp
public sealed record ProductResponse(
    Guid Id,
    string Name,
    decimal Price);
```

O handler da query deve mapear `Product` para `ProductResponse`.

### 10. Criar o controller

Crie `Memo.API/Controllers/ProductsController.cs`, seguindo o padrão de
`CompaniesController`.

O controller deve:

- usar `[ApiController]`;
- usar uma rota plural, como `api/products`;
- receber `CancellationToken ct = default`;
- passar `ct` para `mediator.SendAsync`;
- retornar `Ok`, `CreatedAtAction`, `NoContent` e deixar exceções conhecidas
  para o handler global.

### 11. Criar e aplicar a migration

Após alterar o modelo:

```bash
dotnet ef migrations add CriaProduct \
  --project Memo.Infra \
  --startup-project Memo.API
```

Revise os arquivos gerados e aplique no banco:

```bash
dotnet ef database update \
  --project Memo.Infra \
  --startup-project Memo.API
```

Nunca apague migrations já aplicadas para corrigir o modelo em um ambiente
compartilhado. Crie uma nova migration incremental.

### 12. Criar os testes

Adicione testes para cada camada:

```text
Memo.Domain.Tests/Entities/ProductTests.cs
Memo.Application.Tests/Features/Products/...
Memo.Infra.Tests/Repositories/ProductRepositoryTests.cs
Memo.API.Tests/Controllers/ProductsControllerTests.cs
```

Cubra pelo menos:

- criação válida e inválida;
- atualização válida e inválida;
- limites das regras de negócio;
- entidade inexistente;
- paginação;
- soft-delete;
- índice único, quando aplicável;
- respostas HTTP do controller;
- propagação do `CancellationToken`.

Execute a suíte completa:

```bash
dotnet test Memo.API.sln --no-restore
```

## Checklist de uma nova entidade

- [ ] Entidade criada em `Memo.Domain/Entities`;
- [ ] Entidade herda de `AuditableEntity`, quando deve ter auditoria e soft-delete;
- [ ] Regras de negócio protegidas na entidade;
- [ ] Construtor do EF Core privado;
- [ ] Contrato do repository criado;
- [ ] Todos os métodos assíncronos exigem `CancellationToken`;
- [ ] Configuração do EF Core criada;
- [ ] `DbSet` adicionado ao `AppDbContext`;
- [ ] Repository implementado;
- [ ] Repository registrado no DI;
- [ ] Commands e queries criados;
- [ ] Handlers criados;
- [ ] Validators criados;
- [ ] Responses criados;
- [ ] Controller criado;
- [ ] Migration gerada e aplicada;
- [ ] Testes de domínio, aplicação, infraestrutura e API criados;
- [ ] `dotnet test Memo.API.sln --no-restore` executado com sucesso.
