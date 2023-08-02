namespace CityInfo.API.Services;


public class LocalMailService : IMailService
{
    private readonly string? _mailTo = string.Empty;
    private readonly string? _mailFrom = string.Empty;

    public LocalMailService(IConfiguration configuration)
    {
        _mailFrom = configuration["mailSettings:mailFromAddress"];
        _mailTo = configuration["mailSettings:mailToAddress"];

    }
    public void Send(string subject, string message)
    {
        System.Console.WriteLine($"Mail from {_mailFrom} to {_mailTo}, " +
            $"with {nameof(LocalMailService)}");
        System.Console.WriteLine($"Subject: {subject}");
        System.Console.WriteLine($"Message: {message}");
    }
}