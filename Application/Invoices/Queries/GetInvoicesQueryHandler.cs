using SPS.Application.Common;
using SPS.Application.Invoices.DTOs;
using SPS.Domain.Repositories;

namespace SPS.Application.Invoices.Queries
{
    public class GetInvoicesQueryHandler : IQueryHandler<GetInvoicesQuery, IReadOnlyList<InvoiceDto>>
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public GetInvoicesQueryHandler(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<IReadOnlyList<InvoiceDto>> Handle(GetInvoicesQuery request, CancellationToken cancellationToken)
        {
            IReadOnlyList<Domain.Entities.Invoice> invoices;
            if (request.SubscriptionId.HasValue)
                invoices = await _invoiceRepository.GetBySubscriptionAsync(request.SubscriptionId.Value, cancellationToken);
            else
                invoices = await _invoiceRepository.GetAllAsync(cancellationToken);

            return invoices.Select(i => new InvoiceDto(
                i.Id,
                i.SubscriptionId,
                i.Amount,
                i.IssuedDate,
                i.IsPaid,
                i.PaidDate
            )).ToList();
        }
    }
}
