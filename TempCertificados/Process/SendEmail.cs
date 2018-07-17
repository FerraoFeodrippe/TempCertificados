using System;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace TempCertificados.Process
{
    public class SendEmail
    {
       private readonly IConfiguration _configuration;

        public SendEmail(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task To(string email, string name, byte[] attachment)
        {
            var apiKey = _configuration.GetSection("SENDGRID_API_KEY").Value;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("marcelo@feodrippe.com", "Marcelo");
            var subject = "Teste Even3 - Certificado";
            var to = new EmailAddress(email, name);
            var plainTextContent = "Em anexo segue o seu certificado em pdf.";
            var htmlContent = "<strong>Em anexo segue o seu certificado em pdf.</strong>";
            var attachmentString = Convert.ToBase64String(attachment);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            msg.AddAttachment("Certificado - "+ name +".pdf", attachmentString);
            var response = await client.SendEmailAsync(msg);

        }

    }
}
