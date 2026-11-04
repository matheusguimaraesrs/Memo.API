using Memo.Application.Common.CQRS;
using Memo.Application.Features.Companies.Commands.Delete;
using Memo.Application.Features.Companies.Commands.Update;
using Memo.Application.Features.Companies.Queries.GetAll;
using Memo.Application.Features.Companies.Queries.GetById;
using Memo.Application.Features.Sellers.Commands.Create;
using Microsoft.AspNetCore.Mvc;

namespace Memo.API.Controllers;

[ApiController]
[Route("api/companies")]
public sealed class CompaniesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Listar empresas paginado")]
    [EndpointDescription("Retorna uma lista paginada de empresas ativas.")]
    public async Task<IActionResult> GetAllPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await mediator.SendAsync(new GetAllCompaniesQuery(page, pageSize));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [EndpointSummary("Buscar empresas por ID")]
    [EndpointDescription("Retorna os dados de uma empresa específica.")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await mediator.SendAsync(new GetCompanyByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    [EndpointSummary("Criar empresa")]
    [EndpointDescription("Cria uma empresa nova e retorna o seu ID")]
    public async Task<IActionResult> Create([FromBody] CreateSellerCommand command)
    {
        Guid id = await mediator.SendAsync(command);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    [EndpointSummary("Alterar empresa")]
    [EndpointDescription("Edita os dados de uma empresa específica.")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCompanyCommand command)
    {
        await mediator.SendAsync(command with { Id = id });
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [EndpointSummary("Deletar empresa")]
    [EndpointDescription("Exclui logicamente (soft-delete) uma empresa específica.")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await mediator.SendAsync(new DeleteCompanyCommand(id), ct);
        return NoContent();
    }
}