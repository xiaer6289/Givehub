using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Givehub.Models;

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
        int? donorId = _httpContextAccessor.HttpContext.Session.GetInt32("DonorId");

        if (donorId == null) throw new NotLoggedInException();
        return donorId.Value;
    }

}