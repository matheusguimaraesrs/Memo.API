using Memo.Application.Common.CQRS;
using Memo.Application.Features.Products.Commands.Create;
using Memo.Application.Features.Products.Commands.Delete;
using Memo.Application.Features.Products.Commands.Update;
using Memo.Application.Features.Products.Queries.GetAll;
using Memo.Application.Features.Products.Queries.GetById;
using Microsoft.AspNetCore.Mvc;

namespace Memo.API.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Listar produtos paginado")]
    [EndpointDescription("Retorna uma lista paginada de produtos ativos.")]
    public async Task<IActionResult> GetAllPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var result = await mediator.SendAsync(new GetAllProductsQuery(page, pageSize), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [EndpointSummary("Buscar produtos por ID")]
    [EndpointDescription("Retorna os dados de um produto específico.")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        var result = await mediator.SendAsync(new GetProductByIdQuery(id), ct);
        return Ok(result);
    }

    [HttpPost]
    [EndpointSummary("Criar produto")]
    [EndpointDescription("Cria um produto novo e retorna o seu ID.")]
    public async Task<IActionResult> Create(
        [FromBody] CreateProductCommand command,
        CancellationToken ct = default)
    {
        Guid id = await mediator.SendAsync(command, ct);
        return CreatedAtAction(nameof(GetById), new{ id }, id);
    }

    [HttpPut("{id:guid}")]
    [EndpointSummary("Alterar produto")]
    [EndpointDescription("Edita os dados de um produto específico.")]
    public async Task<IActionResult> Update(
        Guid id, 
        [FromBody] UpdateProductCommand command, 
        CancellationToken ct = default)
    {
        await mediator.SendAsync(command with { Id = id }, ct);
        return NoContent();
    }
    
    [HttpDelete("{id:guid}")]
    [EndpointSummary("Deletar produto")]
    [EndpointDescription("Exclui logicamente (soft-delete) um produto específico.")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        await mediator.SendAsync(new DeleteProductCommand(id), ct);
        return NoContent();
    }
}