using model.validata.com.Enumeration;


namespace model.validata.test.Enumeration
{
    public class BusinessSetOperationTests
    {
        [Fact]
        public void BusinessSetOperation_Enum_HasCorrectValues()
        {
            Assert.Equal(0, (int)BusinessSetOperation.Create);
            Assert.Equal(1, (int)BusinessSetOperation.Update);
            Assert.Equal(2, (int)BusinessSetOperation.Delete);
            Assert.Equal(3, (int)BusinessSetOperation.Get);
        }

        [Fact]
        public void BusinessSetOperation_Enum_CanBeConvertedToString()
        {
            Assert.Equal("Create", BusinessSetOperation.Create.ToString());
            Assert.Equal("Update", BusinessSetOperation.Update.ToString());
            Assert.Equal("Delete", BusinessSetOperation.Delete.ToString());
            Assert.Equal("Get", BusinessSetOperation.Get.ToString());
        }

        [Fact]
        public void BusinessSetOperation_Enum_CanBeParsedFromString()
        {
            Assert.Equal(BusinessSetOperation.Create, Enum.Parse<BusinessSetOperation>("Create"));
            Assert.Equal(BusinessSetOperation.Update, Enum.Parse<BusinessSetOperation>("Update"));
            Assert.Equal(BusinessSetOperation.Delete, Enum.Parse<BusinessSetOperation>("Delete"));
            Assert.Equal(BusinessSetOperation.Get, Enum.Parse<BusinessSetOperation>("Get"));
        }

        [Fact]
        public void BusinessSetOperation_Enum_IsValidForDefinedValues()
        {
            Assert.True(Enum.IsDefined(typeof(BusinessSetOperation), BusinessSetOperation.Create));
            Assert.True(Enum.IsDefined(typeof(BusinessSetOperation), BusinessSetOperation.Update));
            Assert.True(Enum.IsDefined(typeof(BusinessSetOperation), BusinessSetOperation.Delete));
            Assert.True(Enum.IsDefined(typeof(BusinessSetOperation), BusinessSetOperation.Get));
        }

        [Fact]
        public void BusinessSetOperation_Enum_InvalidValueThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => Enum.Parse<BusinessSetOperation>("NonExistentValue"));
        }
    }
}
