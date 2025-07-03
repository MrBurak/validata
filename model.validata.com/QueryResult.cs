using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model.validata.com
{
    public class QueryResult<T>
    {
        public T? Result { get; set; }
        public bool Success { get; set; }
        public string? Exception { get; set; }

    }
}
