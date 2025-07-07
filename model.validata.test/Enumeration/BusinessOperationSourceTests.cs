using model.validata.com.Enumeration;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace model.validata.test.Enumeration
{
    public class BusinessOperationSourceTests
    {
        [Fact]
        public void BusinessOperationSource_Enum_HasCorrectValues()
        {
            Assert.Equal(1, (int)BusinessOperationSource.PreDefined);
            Assert.Equal(2, (int)BusinessOperationSource.Api);
            Assert.Equal(3, (int)BusinessOperationSource.Import);
        }

        [Fact]
        public void BusinessOperationSource_Enum_CanBeConvertedToString()
        {
            Assert.Equal("PreDefined", BusinessOperationSource.PreDefined.ToString());
            Assert.Equal("Api", BusinessOperationSource.Api.ToString());
            Assert.Equal("Import", BusinessOperationSource.Import.ToString());
        }

        [Fact]
        public void BusinessOperationSource_Enum_CanBeParsedFromString()
        {
            Assert.Equal(BusinessOperationSource.PreDefined, Enum.Parse<BusinessOperationSource>("PreDefined"));
            Assert.Equal(BusinessOperationSource.Api, Enum.Parse<BusinessOperationSource>("Api"));
            Assert.Equal(BusinessOperationSource.Import, Enum.Parse<BusinessOperationSource>("Import"));
        }

        [Fact]
        public void BusinessOperationSource_Enum_DisplayNamesAreCorrect()
        {
            Assert.Equal("Pre Defined", GetEnumDisplayName(BusinessOperationSource.PreDefined));
            Assert.Equal("Api", GetEnumDisplayName(BusinessOperationSource.Api));
            Assert.Equal("Import", GetEnumDisplayName(BusinessOperationSource.Import));
        }

        private static string? GetEnumDisplayName(Enum enumValue)
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<DisplayAttribute>()?
                            .Name;
        }

        [Fact]
        public void BusinessOperationSource_Enum_IsValidForDefinedValues()
        {
            Assert.True(Enum.IsDefined(typeof(BusinessOperationSource), BusinessOperationSource.PreDefined));
            Assert.True(Enum.IsDefined(typeof(BusinessOperationSource), BusinessOperationSource.Api));
            Assert.True(Enum.IsDefined(typeof(BusinessOperationSource), BusinessOperationSource.Import));
        }

        [Fact]
        public void BusinessOperationSource_Enum_InvalidValueThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => Enum.Parse<BusinessOperationSource>("NonExistentValue"));
        }
    }
}
