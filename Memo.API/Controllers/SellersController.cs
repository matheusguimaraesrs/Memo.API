using Memo.Application.Common.CQRS;
using Memo.Application.Features.Sellers.Commands.Create;
using Memo.Application.Features.Sellers.Commands.Delete;
using Memo.Application.Features.Sellers.Commands.Update;
using Memo.Application.Features.Sellers.Queries.GetAll;
using Memo.Application.Features.Sellers.Queries.GetById;
using Microsoft.AspNetCore.Mvc;

namespace Memo.API.Controllers;

[ApiController]
[Route("api/sellers")]
public sealed class SellersController(IMediator mediator) : ControllerBase 
{
    [HttpGet]
    [EndpointSummary("Listar vendedores paginado")]
    [EndpointDescription("Retorna a lista paginada de vendedores ativos.")]
    public async Task<IActionResult> GetAllPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await mediator.SendAsync(new GetAllSellersQuery(page, pageSize));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [EndpointSummary("Buscar vendedor por ID")]
    [EndpointDescription("Retorna os dados de um vendedor específico.")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await mediator.SendAsync(new GetSellerByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    [EndpointSummary("Criar vendedor")]
    [EndpointDescription("Cria um vendedor novo e retorna o seu ID.")]
    public async Task<IActionResult> Create([FromBody] CreateSellerCommand command)
    {
        Guid id = await mediator.SendAsync(command);
        return CreatedAtAction(nameof(GetById), new{ id }, id);
    }

    [HttpPut("{id:guid}")]
    [EndpointSummary("Alterar vendedor")]
    [EndpointDescription("Edita os dados de um vendedor específico.")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSellerCommand command)
    {
        await mediator.SendAsync(command with { Id = id });
        return NoContent();
    }
    
    [HttpDelete("{id:guid}")]
    [EndpointSummary("Deletar vendedor")]
    [EndpointDescription("Exclui logicamente (soft-delete) um vendedor específico.")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await mediator.SendAsync(new DeleteSellerCommand(id), ct);
        return NoContent();
    }
}