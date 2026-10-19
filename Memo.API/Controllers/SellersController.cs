using Memo.Application.Common.CQRS;
using Microsoft.AspNetCore.Mvc;

namespace Memo.API.Controllers;

[ApiController]
[Route("api/sellers")]
public sealed class SellersController(IMediator mediator) : ControllerBase 
{
    
}