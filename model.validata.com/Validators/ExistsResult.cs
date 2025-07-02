using model.validata.com.Enumeration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model.validata.com.Validators
{
    public class ExistsResult<TEntity>
    {
        public TEntity? Entity { get; set; }
        public ValidationCode? Code
        { 
            get 
            { 
                return Entity == null ? ValidationCode.RecordNotExists! : null; 
            } 
        }
    }
}
