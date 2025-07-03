using System.ComponentModel.DataAnnotations;
using util.validata.com;

namespace util.validata.test
{
    public class EnumUtilTests
    {
        [Fact]
        public void GetDisplayName_ShouldReturnCorrectDisplayName_WhenAttributeExists()
        {
            
            var enumValue = TestEnumWithDisplay.FirstValue;

            
            string? displayName = EnumUtil.GetDisplayName(enumValue);

            
            Assert.Equal("First Value Display", displayName);
        }

        [Fact]
        public void GetDisplayName_ShouldReturnOnlyDisplayName_WhenOnlyNameExists()
        {
            
            var enumValue = TestEnumWithDisplay.FourthValue;

            
            string? displayName = EnumUtil.GetDisplayName(enumValue);

            
            Assert.Equal("Fourth Value Display", displayName);
        }

        [Fact]
        public void GetDisplayName_ShouldReturnEmptyString_WhenNoDisplayAttribute()
        {
            
            var enumValue = TestEnumWithDisplay.FifthValue;

            
            string? displayName = EnumUtil.GetDisplayName(enumValue);

            
            Assert.Equal(string.Empty, displayName);
        }

        [Fact]
        public void GetDisplayName_ShouldReturnEmptyString_ForEnumWithoutAnyDisplayAttributes()
        {
            
            var enumValue = TestEnumWithoutDisplay.None;

            
            string? displayName = EnumUtil.GetDisplayName(enumValue);

            
            Assert.Equal(string.Empty, displayName);
        }

        [Fact]
        public void GetDisplayDescription_ShouldReturnCorrectDescription_WhenAttributeExists()
        {
            
            var enumValue = TestEnumWithDisplay.SecondValue;

            
            string? description = EnumUtil.GetDisplayDescription(enumValue);

            
            Assert.Equal("Description of Second", description);
        }

        [Fact]
        public void GetDisplayDescription_ShouldReturnEmptyString_WhenNoDescriptionExists()
        {
            
            var enumValue = TestEnumWithDisplay.FourthValue; 
            string? description = EnumUtil.GetDisplayDescription(enumValue);

            
            Assert.Null(description);
        }

        [Fact]
        public void GetDisplayDescription_ShouldReturnEmptyString_WhenNoDisplayAttribute()
        {
            
            var enumValue = TestEnumWithDisplay.FifthValue;

            
            string? description = EnumUtil.GetDisplayDescription(enumValue);

            
            Assert.Equal(string.Empty, description);
        }

        [Fact]
        public void GetDisplayGroupName_ShouldReturnCorrectGroupName_WhenAttributeExists()
        {
            
            var enumValue = TestEnumWithDisplay.ThirdValue;

            
            string? groupName = EnumUtil.GetDisplayGroupName(enumValue);

            
            Assert.Equal("Group A", groupName);
        }

        [Fact]
        public void GetDisplayGroupName_ShouldReturnEmptyString_WhenNoGroupNameExists()
        {
            
            var enumValue = TestEnumWithDisplay.FourthValue; 

            
            string? groupName = EnumUtil.GetDisplayGroupName(enumValue);

            
            Assert.Null(groupName);
        }

        [Fact]
        public void GetDisplayGroupName_ShouldReturnEmptyString_WhenNoDisplayAttribute()
        {
            
            var enumValue = TestEnumWithDisplay.FifthValue;

            
            string? groupName = EnumUtil.GetDisplayGroupName(enumValue);

            
            Assert.Equal(string.Empty, groupName);
        }

        [Fact]
        public void EnumToObject_ShouldMapAllPropertiesCorrectly()
        {
            
            var enumValue = TestEnumWithDisplay.SecondValue;

            
            var nameValue = EnumUtil.EnumToObject(enumValue);

            
            Assert.NotNull(nameValue);
            Assert.Equal("SecondValue", nameValue.Name);
            Assert.Equal(2, nameValue.Value);
            Assert.Equal("Second Value Display", nameValue.DisplayName);
            Assert.Equal("Description of Second", nameValue.DisplayDescription);
            Assert.Equal("Group B", nameValue.DisplayGroupName);
        }

        [Fact]
        public void EnumToObject_ShouldHandleEnumWithoutDisplayAttribute()
        {
            
            var enumValue = TestEnumWithoutDisplay.One;

            
            var nameValue = EnumUtil.EnumToObject(enumValue);

            
            Assert.NotNull(nameValue);
            Assert.Equal("One", nameValue.Name);
            Assert.Equal(1, nameValue.Value);
            Assert.Equal(string.Empty, nameValue.DisplayName);
            Assert.Equal(string.Empty, nameValue.DisplayDescription);
            Assert.Equal(string.Empty, nameValue.DisplayGroupName);
        }


        [Fact]
        public void EnumToList_ShouldReturnCorrectListOfNameValueObjects()
        {
            
            var list = EnumUtil.EnumToList<TestEnumWithDisplay>();

            
            Assert.NotNull(list);
            Assert.Equal(5, list.Count); 

            var first = list.First(nv => nv.Name == "FirstValue");
            Assert.Equal("First Value Display", first.DisplayName);
            Assert.Equal("Description of First", first.DisplayDescription);
            Assert.Equal("Group A", first.DisplayGroupName);
            Assert.Equal(1, first.Value);

            var fifth = list.First(nv => nv.Name == "FifthValue");
            Assert.Equal(string.Empty, fifth.DisplayName);
            Assert.Equal(string.Empty, fifth.DisplayDescription);
            Assert.Equal(string.Empty, fifth.DisplayGroupName);
            Assert.Equal(5, fifth.Value);
        }

        [Fact]
        public void EnumToList_ShouldReturnCorrectListOfNameValueObjectsForEnumWithoutDisplayAttributes()
        {
            
            var list = EnumUtil.EnumToList<TestEnumWithoutDisplay>();

            
            Assert.NotNull(list);
            Assert.Equal(2, list.Count);

            var none = list.First(nv => nv.Name == "None");
            Assert.Equal("None", none.Name);
            Assert.Equal(0, none.Value);
            Assert.Equal(string.Empty, none.DisplayName);
            Assert.Equal(string.Empty, none.DisplayDescription);
            Assert.Equal(string.Empty, none.DisplayGroupName);
        }

        [Fact]
        public void EnumToList_ShouldThrowArgumentException_WhenTypeIsNotAnEnum()
        {
            
            Assert.Throws<ArgumentException>(() => EnumUtil.EnumToList<string>());
            Assert.Throws<ArgumentException>(() => EnumUtil.EnumToList<int>());
            Assert.Throws<ArgumentException>(() => EnumUtil.EnumToList<object>());
        }

        [Theory]
        [InlineData(1, "Group A")]
        [InlineData(2, "Group B")]
        [InlineData(3, "Group A")]
        public void FindGroupNameByValueId_ShouldReturnCorrectGroupName(int value, string expectedGroupName)
        {
            
            string actualGroupName = EnumUtil.FindGroupNameByValueId<TestEnumWithDisplay>(value);

            
            Assert.Equal(expectedGroupName, actualGroupName);
        }

        [Fact]
        public void FindGroupNameByValueId_ShouldReturnEmptyStringForValueWithNoGroupName()
        {
          
            int value = 4;

            
            string actualGroupName = EnumUtil.FindGroupNameByValueId<TestEnumWithDisplay>(value);

            
            Assert.Null(actualGroupName);
        }

        [Fact]
        public void FindGroupNameByValueId_ShouldReturnEmptyStringForValueWithNoDisplayAttribute()
        {
            int value = 5;

            
            string actualGroupName = EnumUtil.FindGroupNameByValueId<TestEnumWithDisplay>(value);

            
            Assert.Equal(string.Empty, actualGroupName);
        }

        [Fact]
        public void FindGroupNameByValueId_ShouldThrowExceptionForUnsupportedValue()
        {
            
            int unsupportedValue = 99;

            
            var ex = Assert.Throws<Exception>(() => EnumUtil.FindGroupNameByValueId<TestEnumWithDisplay>(unsupportedValue));
            Assert.Contains($"Unsupported value for {typeof(TestEnumWithDisplay).FullName} : {unsupportedValue}", ex.Message);
        }

        [Fact]
        public void FindGroupNameByValueId_ShouldThrowExceptionForEnumWithoutDisplayAttributes()
        {
            
            int value = 0; 

            
            string actualGroupName = EnumUtil.FindGroupNameByValueId<TestEnumWithoutDisplay>(value);
            Assert.Equal(string.Empty, actualGroupName);

            int unsupportedValue = 99;
            var ex = Assert.Throws<Exception>(() => EnumUtil.FindGroupNameByValueId<TestEnumWithoutDisplay>(unsupportedValue));
            Assert.Contains($"Unsupported value for {typeof(TestEnumWithoutDisplay).FullName} : {unsupportedValue}", ex.Message);
        }
    }
    public enum TestEnumWithDisplay
    {
        [Display(Name = "First Value Display", Description = "Description of First", GroupName = "Group A")]
        FirstValue = 1,

        [Display(Name = "Second Value Display", Description = "Description of Second", GroupName = "Group B")]
        SecondValue = 2,

        [Display(Name = "Third Value Display", Description = "Description of Third", GroupName = "Group A")]
        ThirdValue = 3,

        [Display(Name = "Fourth Value Display")] 
        FourthValue = 4,

        FifthValue = 5 
    }

    public enum TestEnumWithoutDisplay
    {
        None = 0,
        One = 1
    }
}
