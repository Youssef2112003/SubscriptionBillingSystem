namespace SPS.Application.Invoices.DTOs
{
    public record InvoiceDto(
    Guid Id,
    Guid SubscriptionId,
    decimal Amount,
    DateTime IssuedDate,
    bool IsPaid,
    DateTime? PaidDate
);
}
