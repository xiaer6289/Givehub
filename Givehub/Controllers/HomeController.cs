using Givehub.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;

namespace Givehub.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }

    [HttpPost]
    public IActionResult SendContactForm(ContactFormDTO form)
    {
        string toEmail = "xiaer6289@gmail.com";
        string fromEmail = "noreply@yourdomain.com";

        string needsList = form.Needs != null ? string.Join(", ", form.Needs) : "None";

        var message = new MailMessage();
        message.From = new MailAddress(fromEmail);
        message.To.Add(toEmail);
        message.Subject = "Contact Form Submission";
        message.Body =
            $"Organization Name: {form.Name}\n" +
            $"Phone Number: {form.PhoneNo}\n" +
            $"Email: {form.Email}\n" +
            $"Needs: {needsList}";

        using (var client = new SmtpClient("sandbox.smtp.mailtrap.io", 2525))
        {
            client.Credentials = new NetworkCredential("", "");
            client.EnableSsl = true;
            client.Send(message);
        };

        TempData["Success"] = "Your Request has been sent successfully!";
        return RedirectToAction("Contact");
    }

    public IActionResult Donation()
    {
        return View();
    }
}
