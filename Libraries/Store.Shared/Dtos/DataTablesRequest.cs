using System.Collections.Generic;

namespace Store.Shared.Dtos
{
    public class DataTablesRequest
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public DtSearch Search { get; set; }
        public List<DtOrder> Order { get; set; }
        public List<DtColumn> Columns { get; set; }
    }

    public class DtSearch
    {
        public string Value { get; set; }
        public bool Regex { get; set; }
    }

    public class DtOrder
    {
        public int Column { get; set; }
        public string Dir { get; set; }
    }

    public class DtColumn
    {
        public string Data { get; set; }
        public string Name { get; set; }
        public bool Searchable { get; set; }
        public bool Orderable { get; set; }
        public DtSearch Search { get; set; }
    }
}
