using MediatR;
using SPS.Application.Common;

namespace SPS.Application.Invoices.Commands
{
    public record PayInvoiceCommand(Guid InvoiceId) : ICommand<Unit>;
}
