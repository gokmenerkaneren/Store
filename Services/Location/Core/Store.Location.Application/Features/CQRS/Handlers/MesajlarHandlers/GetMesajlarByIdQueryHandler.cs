using Store.Location.Application.Features.CQRS.Queries.MesajlarQueries;
using Store.Location.Application.Features.CQRS.Results.MesajlarResults;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;
using System.Threading.Tasks;

namespace Store.Location.Application.Features.CQRS.Handlers.MesajlarHandlers
{
    public class GetMesajlarByIdQueryHandler
    {
        private readonly IRepository<Mesajlar> _repository;

        public GetMesajlarByIdQueryHandler(IRepository<Mesajlar> repository)
        {
            _repository = repository;
        }

        public async Task<GetMesajlarByIdQueryResult> Handle(GetMesajlarByIdQuery query)
        {
            var values = await _repository.GetByIdAsync(query.Id);
            return new GetMesajlarByIdQueryResult
            {
                Id = values.Id,
                NameSurname = values.NameSurname,
                Email = values.Email,
                Phone = values.Phone,
                Subject = values.Subject,
                Message = values.Message,
                IsRead = values.IsRead,
                MessageDate = values.MessageDate
            };
        }
    }
}
