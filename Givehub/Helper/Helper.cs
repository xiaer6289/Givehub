using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Givehub.Models;

namespace Givehub.Helper;

public class Helper : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly DB _db;
    
    public Helper(IHttpContextAccessor httpContextAccessor, DB db)
    {
        _httpContextAccessor = httpContextAccessor;
        _db = db;
    }


}
