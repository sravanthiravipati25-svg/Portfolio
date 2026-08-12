using Microsoft.AspNetCore.Mvc;
using Portfolio.Models;
using Portfolio.Services;

namespace Portfolio.Controllers;

public class HomeController : Controller
{
    private readonly IEmailService _emailService;

    public HomeController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new ContactFormModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Contact(ContactFormModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please fix the validation errors and try again.";
            return View("Index", model);
        }

        try
        {
            await _emailService.SendAsync(model.Name, model.Email, model.Message);

            TempData["Success"] = "Your message has been sent successfully. I’ll get back to you soon.";
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            TempData["Error"] = "Sorry, something went wrong while sending your message.";
            return View("Index", model);
        }
}
}
