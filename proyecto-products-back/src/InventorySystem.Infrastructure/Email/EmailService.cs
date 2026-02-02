using System.Net;
using System.Net.Mail;
using InventorySystem.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace InventorySystem.Infrastructure.Email;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        try
        {
            using var client = new SmtpClient(_settings.SmtpServer, _settings.SmtpPort)
            {
                Credentials = new NetworkCredential(_settings.Username, _settings.Password),
                EnableSsl = _settings.EnableSsl
            };

            var message = new MailMessage
            {
                From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            message.To.Add(to);

            await client.SendMailAsync(message, cancellationToken);
            _logger.LogInformation("Email enviado a {To}: {Subject}", to, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enviando email a {To}", to);
        }
    }

    public async Task SendLowStockAlertAsync(string to, string productName, int stockQuantity, CancellationToken cancellationToken = default)
    {
        var subject = $"⚠️ Alerta: Stock bajo - {productName}";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #e53e3e;'>⚠️ Alerta de Stock Bajo</h2>
                    <p>Se ha detectado un producto con stock bajo en el sistema de inventarios.</p>
                    <div style='background: #fff5f5; border-left: 4px solid #e53e3e; padding: 15px; margin: 20px 0;'>
                        <p><strong>Producto:</strong> {productName}</p>
                        <p><strong>Stock actual:</strong> {stockQuantity} unidades</p>
                    </div>
                    <p>Por favor, revise el inventario y realice el reabastecimiento necesario.</p>
                    <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;'>
                    <p style='color: #888; font-size: 12px;'>Sistema de Gestión de Inventarios - Touch Consulting</p>
                </div>
            </body>
            </html>";

        await SendEmailAsync(to, subject, body, cancellationToken);
    }
}
