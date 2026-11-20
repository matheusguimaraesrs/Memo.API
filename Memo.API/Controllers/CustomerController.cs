using Memo.Application.Common.CQRS;
using Memo.Application.Features.Customers.Commands.Create;
using Memo.Application.Features.Customers.Commands.Delete;
using Memo.Application.Features.Customers.Commands.Update;
using Memo.Application.Features.Customers.Queries.GetAll;
using Memo.Application.Features.Customers.Queries.GetById;
using Microsoft.AspNetCore.Mvc;

namespace Memo.API.Controllers;

[ApiController]
[Route("api/customers")]
public sealed class CustomerController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Listar clientes paginado")]
    [EndpointDescription("Retorna uma lista paginada de clientes ativos.")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var result = await mediator.SendAsync(new GetAllCustomersQuery(page, pageSize), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [EndpointSummary("Buscar cliente por ID")]
    [EndpointDescription("Retorna os dados de um cliente específico.")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        var result = await mediator.SendAsync(new GetCustomerByIdQuery(id), ct);
        return Ok(result);
    }

    [HttpPost]
    [EndpointSummary("Criar cliente")]
    [EndpointDescription("Cria um cliente novo e retorna o seu ID.")]
    public async Task<IActionResult> Create(
        [FromBody] CreateCustomerCommand command,
        CancellationToken ct = default)
    {
        Guid id = await mediator.SendAsync(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }
    
    [HttpPut("{id:guid}")]
    [EndpointSummary("Alterar cliente")]
    [EndpointDescription("Edita os dados de um cliente específico.")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateCustomerCommand command,
        CancellationToken ct = default)
    {
        await mediator.SendAsync(command with { Id = id }, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [EndpointSummary("Deletar cliente")]
    [EndpointDescription("Exclui logicamente (soft-delete) um cliente específico.")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        await mediator.SendAsync(new DeleteCustomerCommand(id), ct);
        return NoContent();
    }
}