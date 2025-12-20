using System.Security.Claims;
using System.Text.Json;
using Givehub.Models;
using Microsoft.AspNetCore.Mvc;

namespace Givehub.Helpers;

public class Helper
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly DB _db;

    public Helper(IHttpContextAccessor httpContextAccessor, DB db)
    {
        _httpContextAccessor = httpContextAccessor;
        _db = db;
    }

    public int GetLoggedDonorId()
    {
        var claimValue = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(claimValue))
            throw new NotLoggedInException();

        return int.Parse(claimValue);
    }

}