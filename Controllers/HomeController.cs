using Microsoft.AspNetCore.Mvc;
using Portfolio.Models;
using Portfolio.Services;

namespace Portfolio.Controllers;

public class HomeController : Controller
{
    private readonly IEmailService _emailService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IEmailService emailService, ILogger<HomeController> logger)
    {
        _emailService = emailService;
        _logger = logger;
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
        _logger.LogInformation("Contact form submitted from {Email}", model.Email);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Contact form validation failed");
            TempData["Error"] = "Please fix the validation errors and try again.";
            return View("Index", model);
        }

        try
        {
            _logger.LogInformation("Calling EmailService...");

            await _emailService.SendAsync(model.Name, model.Email, model.Message);

            _logger.LogInformation("EmailService completed successfully");

            TempData["Success"] =
                "Thank you! Your message has been sent successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Contact form email failed");

            TempData["Error"] =
                "Sorry, the message could not be sent right now.";

            return RedirectToAction(nameof(Index));
        }
    }
}

