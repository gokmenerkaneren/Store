namespace Store.Location.Application.Features.CQRS.Commands.CompanyCommands
{
    public class DeleteCompanyCommand
    {
        public Guid Id { get; set; }
        public DeleteCompanyCommand(Guid id) => Id = id;
    }
}
