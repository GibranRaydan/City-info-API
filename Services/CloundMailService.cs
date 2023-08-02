namespace CityInfo.API.Services;


public class CloudMailService : IMailService
{
    private string _mailTo = "gibran@email.com";
    private string _mailFrom = "jaime@eymail.co";

    public void Send(string subject, string message)
    {
        System.Console.WriteLine($"Mail from {_mailFrom} to {_mailTo}," +
            $"with {nameof(CloudMailService)}");
        System.Console.WriteLine($"Subject: {subject}");
        System.Console.WriteLine($"Message: {message}");
    }
}