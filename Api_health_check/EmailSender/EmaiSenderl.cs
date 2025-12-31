using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Api_health_check.Email
{
    public interface IEmailSender
    {
        Task SendAsync(string to, string subject, string body);
    }

    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public SmtpEmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            var host = _config["Email:Host"];
            var port = int.Parse(_config["Email:Port"]!);
            var useSsl = bool.Parse(_config["Email:UseSsl"]!);
            var user = _config["Email:UserName"];
            var pass = _config["Email:Password"];
            var from = _config["Email:From"];

            using var client = new SmtpClient(host!, port)
            {
                EnableSsl = useSsl,
                Credentials = new NetworkCredential(user, pass)
            };

            using var message = new MailMessage(from!, to, subject, body);
            await client.SendMailAsync(message);
        }
    }
}
