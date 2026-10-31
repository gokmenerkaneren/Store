using System;

namespace Store.Location.Application.Features.CQRS.Results.MesajlarResults
{
    public class GetMesajlarQueryResult
    {
        public Guid Id { get; set; }
        public string NameSurname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime MessageDate { get; set; }
    }
}
