using Store.Location.Application.Features.CQRS.Queries.MesajlarQueries;
using Store.Location.Application.Features.CQRS.Results.MesajlarResults;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Location.Application.Features.CQRS.Handlers.MesajlarHandlers
{
    public class GetMesajlarQueryHandler
    {
        private readonly IRepository<Mesajlar> _repository;

        public GetMesajlarQueryHandler(IRepository<Mesajlar> repository)
        {
            _repository = repository;
        }

        public async Task<List<GetMesajlarQueryResult>> Handle(bool showDeleted = false)
        {
            var values = await _repository.GetAllAsync(showDeleted);
            return values.Select(x => new GetMesajlarQueryResult
            {
                Id = x.Id,
                NameSurname = x.NameSurname,
                Email = x.Email,
                Phone = x.Phone,
                Subject = x.Subject,
                Message = x.Message,
                IsRead = x.IsRead,
                MessageDate = x.MessageDate
            }).OrderByDescending(x => x.MessageDate).ToList();
        }
    }
}
