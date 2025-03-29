using System.Net;
using System.Net.Mail;

namespace api.Services
{
public class EmailService
{
    private readonly string _smtpServer = "smtp.gmail.com";
    private readonly int _port = 587;
    private readonly string _emailFrom = "kfoods68@gmail.com";
    private readonly string _password = "fodr zkyy qsot flos";

   public async Task SendPasswordResetEmailAsync(string toEmail, string token)
    {
        var smtpClient = ConfigureSmtpClient();
        var mailMessage = CreatePasswordResetEmail(toEmail, token);
        await smtpClient.SendMailAsync(mailMessage);
    }

    public async Task SendVerificationEmailAsync(string toEmail)
    {
        var smtpClient = ConfigureSmtpClient();
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_emailFrom, "KFoods"),
            Subject = "Registro exitoso",
            Body = "Tu cuenta ha sido registrada exitosamente. Gracias por unirte a KFoods!",
            IsBodyHtml = true,
        };
        mailMessage.To.Add(toEmail);
        await smtpClient.SendMailAsync(mailMessage);
    }


    private SmtpClient ConfigureSmtpClient()
    {
        return new SmtpClient(_smtpServer)
        {
            Port = _port,
            Credentials = new NetworkCredential(_emailFrom, _password),
            EnableSsl = true,
        };
    }

    private MailMessage CreatePasswordResetEmail(string toEmail, string token)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_emailFrom, "KFoods"),
            Subject = "Restablecimiento de contraseña",
            Body = $"Tu código de restablecimiento es: {token}",
            IsBodyHtml = true,
        };
        mailMessage.Headers.Add("X-Priority", "1");  // Prioridad alta
        mailMessage.Headers.Add("Importance", "High");  // Alta importancia
        mailMessage.To.Add(toEmail);
        return mailMessage;
    }

}
}