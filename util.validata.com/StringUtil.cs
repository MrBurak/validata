using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace util.validata.com
{
    public class StringUtil
    {
        public static bool IsEmpty(string? val)
        {
            return string.IsNullOrWhiteSpace(val);
        }
    }
}
