namespace BackOffice.Domain.Users
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
