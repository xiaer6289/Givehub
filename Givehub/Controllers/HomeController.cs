using Givehub.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;


namespace Givehub.Controllers;

public class HomeController : Controller
{
    private readonly DB _context;
    public HomeController(DB context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Contact(ContactFormDTO form)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please fill in all fields correctly";
            return View(form);
        }
        string toEmail = "johndoe@gmail.com";
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
            client.Credentials = new NetworkCredential("dd4e1422eddfcd", "e30e080db30291");
            client.EnableSsl = true;
            client.Send(message);
        };

        TempData["Success"] = "Your Request has been sent successfully!";
        return RedirectToAction("Contact");
    }

    public IActionResult Donation(string search, string category, int page = 1)
    {
        int pageSize = 6;
        var query =_context.Donees.AsQueryable();

        if(!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            query = query.Where(d=>
            d.Name.ToLower().Contains(search)||
            d.Address.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(d => d.Category == category);
        }

        int totalItems = query.Count();
        int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var donees = query
            .OrderBy(d => d.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.Search = search;
        ViewBag.Category = category;

        return View(donees);
    }

}
