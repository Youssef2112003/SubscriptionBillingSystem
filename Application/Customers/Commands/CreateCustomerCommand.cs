using SPS.Application.Common;

namespace SPS.Application.Customers.Commands
{
    public record CreateCustomerCommand(string Name, string Email) : ICommand<Guid>;

}
