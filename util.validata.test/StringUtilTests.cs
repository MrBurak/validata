using System.Text.RegularExpressions;
using util.validata.com;

namespace util.validata.test
{
    public class StringUtilTests
    {
        [Theory]
        [InlineData(null, true)]
        [InlineData("", true)]
        [InlineData("   ", true)]
        [InlineData("hello", false)]
        [InlineData(" hello ", false)]
        public void IsEmpty_ShouldReturnCorrectly(string val, bool expected)
        {
            bool actual = StringUtil.IsEmpty(val);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("HelloWorld123", null, true)] 
        [InlineData("Hello World", null, true)] 
        [InlineData("Hello-World_123", null, true)]
        [InlineData("Hello!World", null, false)]
        [InlineData("12345", null, true)]
        [InlineData("你好世界", null, true)] 
        [InlineData("test@example.com", @"^[a-zA-Z0-9@.]+$", true)] 
        [InlineData("test!example", @"^[a-zA-Z0-9@.]+$", false)] 
        [InlineData("", null, false)]  
        [InlineData("a", null, true)]
        public void IsAlphaNumeric_ShouldReturnCorrectly(string val, string regularExpression, bool expected)
        {
            bool actual = StringUtil.IsAlphaNumeric(val, regularExpression);


            Assert.Equal(expected, actual);
        }

        

        [Theory]
        [InlineData("[a-z", "abc")] 
        public void IsAlphaNumeric_ShouldThrowRegexParseExceptionForInvalidRegex(string invalidRegex, string val)
        {

            Assert.Throws<RegexParseException>(() => StringUtil.IsAlphaNumeric(val, invalidRegex));
        }
    }
}