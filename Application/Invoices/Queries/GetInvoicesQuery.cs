using SPS.Application.Common;
using SPS.Application.Invoices.DTOs;

namespace SPS.Application.Invoices.Queries
{
    public record GetInvoicesQuery(Guid? SubscriptionId = null, int Page = 1, int PageSize = 10)
    : IQuery<IReadOnlyList<InvoiceDto>>;
}
