using Microsoft.AspNetCore.Mvc;

namespace Givehub.Models;

public class StripeSettings
{
    public string SecretKey { get; set; }
    public string PublicKey { get; set; }
}
