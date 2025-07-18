﻿using System.Text.RegularExpressions;

namespace util.validata.com
{
    public class StringUtil
    {
        private const string defaultRegularExpression = @"^[\p{L}0-9-_ ]+$";
        public static bool IsEmpty(string? val)
        {
            return string.IsNullOrWhiteSpace(val);
        }

        public static bool IsAlphaNumeric(string val, string? regularExpression)
        {
            
            Regex rg = new Regex(regularExpression??defaultRegularExpression);
            return rg.IsMatch(val);
        }
    }
}
