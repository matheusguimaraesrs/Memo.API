using System.Security.Claims;
using Memo.Domain.Common;
using Microsoft.AspNetCore.Http;

namespace Memo.Infra.Services;

public class UserContext(IHttpContextAccessor accessor) : IUserContext
{
    public string? Id => accessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    public string? Name => accessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
}