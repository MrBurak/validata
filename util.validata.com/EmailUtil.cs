

using System.Net.Mail;

namespace util.validata.com
{
    public class EmailUtil
    {
        public static bool IsValid(string? emailaddress)
        {
            if(StringUtil.IsEmpty(emailaddress))return false;
            try
            {
                MailAddress m = new MailAddress(emailaddress!);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
