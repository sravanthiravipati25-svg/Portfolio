namespace Portfolio.Services;

public interface IEmailService
{
    Task SendAsync(string name, string email, string message);
}