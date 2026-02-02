namespace InventorySystem.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
    Task SendLowStockAlertAsync(string to, string productName, int stockQuantity, byte[]? pdfAttachment = null, CancellationToken cancellationToken = default);
}
