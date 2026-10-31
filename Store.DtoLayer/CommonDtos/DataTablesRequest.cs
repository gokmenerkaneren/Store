using System.Collections.Generic;

namespace Store.DtoLayer.CommonDtos
{
    public class DataTablesRequest
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; } = 10;
        public DataTablesSearch Search { get; set; } = new();
        public List<DataTablesOrder> Order { get; set; } = new();
        public List<DataTablesColumn> Columns { get; set; } = new();
    }

    public class DataTablesSearch
    {
        public string? Value { get; set; }
        public bool Regex { get; set; }
    }

    public class DataTablesOrder
    {
        public int Column { get; set; }
        public string Dir { get; set; } = "asc";
    }

    public class DataTablesColumn
    {
        public string? Data { get; set; }
        public string? Name { get; set; }
        public bool Searchable { get; set; }
        public bool Orderable { get; set; }
        public DataTablesSearch Search { get; set; } = new();
    }
}
