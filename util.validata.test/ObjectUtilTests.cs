using System.Linq.Expressions;
using util.validata.com; 
namespace util.validata.test
{
    public class ObjectUtilTests
    {
        

        
        [Fact]
        public void ConcatLambdaExpression_ShouldCombineTwoExpressionsWithAndAlso()
        {
            Expression<Func<TestObject, bool>> firstExpression = o => o.IntProperty > 100;
            Expression<Func<TestObject, bool>> secondExpression = o => o.StringProperty.StartsWith("h");

            var testObj1 = new TestObject("hello", 150, true);  
            var testObj2 = new TestObject("world", 50, true);   
            var testObj3 = new TestObject("hi", 50, true);      
            var testObj4 = new TestObject("hello", 200, true);  
            
            var combinedExpression = ObjectUtil.ConcatLambdaExpression(firstExpression, secondExpression);
            var compiledExpression = combinedExpression.Compile();

            Assert.True(compiledExpression(testObj1));
            Assert.False(compiledExpression(testObj2));
            Assert.False(compiledExpression(testObj3));
            Assert.True(compiledExpression(testObj4));
        }

        [Fact]
        public void ConcatLambdaExpression_ShouldWorkWithDifferentParameterNames()
        {
           
            Expression<Func<TestObject, bool>> firstExpression = item => item.IntProperty == 123;
            Expression<Func<TestObject, bool>> secondExpression = obj => obj.BoolProperty == true;

            var testObj1 = new TestObject("a", 123, true); 
            var testObj2 = new TestObject("b", 456, true); 
            var testObj3 = new TestObject("c", 123, false); 

            var combinedExpression = ObjectUtil.ConcatLambdaExpression(firstExpression, secondExpression);
            var compiledExpression = combinedExpression.Compile();

            Assert.True(compiledExpression(testObj1));
            Assert.False(compiledExpression(testObj2));
            Assert.False(compiledExpression(testObj3));
        }


        [Fact]
        public void NotContainsLambdaExpression_ShouldFilterOutObjectsWithMatchingId()
        {
            List<int> excludedIds = new List<int> { 1, 3, 5 };
            var obj1 = new SourceObject { Id = 1, Name = "Test1" };
            var obj2 = new SourceObject { Id = 2, Name = "Test2" };
            var obj3 = new SourceObject { Id = 3, Name = "Test3" };
            var obj4 = new SourceObject { Id = 4, Name = "Test4" };

            var notContainsExpression = ObjectUtil.NotContainsLambdaExpression<SourceObject>(excludedIds, "Id");
            var compiledExpression = notContainsExpression.Compile();

            Assert.False(compiledExpression(obj1)); 
            Assert.True(compiledExpression(obj2));  
            Assert.False(compiledExpression(obj3)); 
            Assert.True(compiledExpression(obj4));  
        }

        [Fact]
        public void NotContainsLambdaExpression_ShouldHandleEmptyIdList()
        {
            List<int> excludedIds = new List<int>();
            var obj1 = new SourceObject { Id = 1, Name = "Test1" };

            var notContainsExpression = ObjectUtil.NotContainsLambdaExpression<SourceObject>(excludedIds, "Id");
            var compiledExpression = notContainsExpression.Compile();

            Assert.True(compiledExpression(obj1)); 
        }

        [Fact]
        public void NotContainsLambdaExpression_ShouldHandleNonIntegerKeyProperty()
        {
            
            List<int> excludedIds = new List<int> { 1, 2 }; 
            var obj1 = new SourceObject { Id = 1, Name = "Test1" };

           
            Assert.Throws<ArgumentException>(() => ObjectUtil.NotContainsLambdaExpression<SourceObject>(excludedIds, "Name"));
        }

        
       
    }

    public class TestObject
    {
        public string StringProperty { get; set; }
        public int IntProperty { get; set; }
        public bool BoolProperty { get; set; }
        public string? NullProperty { get; set; }
        public string PrivateProperty { get; private set; } 

        public TestObject(string stringVal, int intVal, bool boolVal)
        {
            StringProperty = stringVal;
            IntProperty = intVal;
            BoolProperty = boolVal;
            PrivateProperty = "private";
        }
    }

    

    public class SourceObject
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class TargetObject
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal Price { get; set; } 
    }
}
