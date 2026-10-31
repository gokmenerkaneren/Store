namespace Store.Location.Application.Features.CQRS.Commands.CompanyCommands
{
    public class RemoveCompanyCommand
    {
        public Guid Id { get; set; }
        public RemoveCompanyCommand(Guid id) => Id = id;
    }
}
