namespace Store.Location.Application.Features.CQRS.Commands.CompanyCommands
{
    public class SoftDeleteCompanyCommand
    {
        public Guid Id { get; set; }
        public SoftDeleteCompanyCommand(Guid id) => Id = id;
    }
}
