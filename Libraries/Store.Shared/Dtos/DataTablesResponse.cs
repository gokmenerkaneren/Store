using System.Collections.Generic;

namespace Store.Shared.Dtos
{
    public class DataTablesResponse<T>
    {
        public int Draw { get; set; }
        public long RecordsTotal { get; set; }
        public long RecordsFiltered { get; set; }
        public IEnumerable<T> Data { get; set; }
        public string Error { get; set; }
    }
}
