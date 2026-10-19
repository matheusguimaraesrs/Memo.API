using Memo.Application.Common.CQRS;
using Memo.Application.Features.Sellers.Queries.GetAll;
using Microsoft.AspNetCore.Mvc;

namespace Memo.API.Controllers;

[ApiController]
[Route("api/sellers")]
public sealed class SellersController(IMediator mediator) : ControllerBase 
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await mediator.SendAsync(new GetAllSellersQuery());
        return Ok(result);
    }
}