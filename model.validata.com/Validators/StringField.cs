using model.validata.com.Enumeration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model.validata.com.Validators
{
    public class StringField<TEntity>
    {
        public TEntity? Entity { get; set; }
        public string? Field { get; set; }
        public bool CheckRegex { get; set; }
        public string? Regex { get; set; }
        public bool CheckUnique { get; set; }
        public string EmptyMesssage { get { return $"Field {Field} is required"; } }
        public string RegexMesssage { get { return $"Field {Field} is invalid"; } }
        public string UnixMesssage { get { return $"Field {Field} value belongs to an other record in {typeof(TEntity)}"; } }

        public List<int> Ids { get; set; } = new List<int>();

    }
}
