using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model.validata.com.Enumeration
{
    public enum ValidationCode
    {
        RecordNotExists,
        FirstIsRequired,
        LastIsRequired,
        EmailAddressInvalid,
        PhoneIsInvalid,
        PoboxIsInvalid,
    }
}
