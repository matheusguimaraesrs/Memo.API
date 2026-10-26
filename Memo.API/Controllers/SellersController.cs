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
    public async Task<IActionResult> GetAllPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await mediator.SendAsync(new GetAllSellersQuery(page, pageSize));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await mediator.SendAsync(new GetSellerByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSellerCommand command)
    {
        Guid id = await mediator.SendAsync(command);
        return CreatedAtAction(nameof(GetById), new{ id }, id);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSellerCommand command)
    {
        await mediator.SendAsync(command with { Id = id });
        return NoContent();
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await mediator.SendAsync(new DeleteSellerCommand(id), ct);
        return NoContent();
    }
}