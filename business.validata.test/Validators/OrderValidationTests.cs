using business.validata.com.Interfaces.Validators;
using business.validata.com.Validators;
using FluentAssertions;
using model.validata.com.Entities;
using model.validata.com.Enumeration;
using model.validata.com.Validators;
using model.validata.com.ValueObjects.Customer;
using model.validata.com.ValueObjects.Order;
using model.validata.com.ValueObjects.OrderItem;
using Moq;

namespace business.validata.test.Validators
{


    public class OrderValidationTests
    {
        private readonly Mock<IGenericValidation<Customer>> customerValidationMock = new();
        private readonly Mock<IGenericValidation<Product>> productValidationMock = new();
        private readonly Mock<IGenericValidation<Order>> orderValidationMock = new();

        private readonly OrderValidation sut;

        public OrderValidationTests()
        {
            sut = new OrderValidation(
                customerValidationMock.Object,
                productValidationMock.Object,
                orderValidationMock.Object
            );
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenCustomerValidationIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new OrderValidation(null!, productValidationMock.Object, orderValidationMock.Object));
            Assert.Contains("genericValidationCustomer", ex.ParamName);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenProductValidationIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new OrderValidation(customerValidationMock.Object, null!, orderValidationMock.Object));
            Assert.Contains("genericValidationProduct", ex.ParamName);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenOrderValidationIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new OrderValidation(customerValidationMock.Object, productValidationMock.Object, null!));
            Assert.Contains("genericValidationOrder", ex.ParamName);
        }

        [Fact]
        public void Constructor_Succeeds_WhenAllDependenciesProvided()
        {
            var sut = new OrderValidation(customerValidationMock.Object, productValidationMock.Object, orderValidationMock.Object);
            Assert.NotNull(sut);
        }

        [Fact]
        public async Task Should_ReturnError_IfCustomerNotFound()
        {
            var order = CreateOrder();
            customerValidationMock.Setup(m => m.Exists(order.CustomerId, BusinessSetOperation.Get))
                .ReturnsAsync(new ExistsResult<Customer> { Entity = null });

            var result = await sut.InvokeAsync(order, BusinessSetOperation.Create);

            result.ValidationResult.IsValid.Should().BeFalse();
            result.ValidationResult.Errors.Should().Contain("Customer Not Found");
        }

        [Fact]
        public async Task Should_ReturnError_IfOrderNotFoundOnUpdate()
        {
            var order = CreateOrder();
            orderValidationMock.Setup(m => m.Exists(order.OrderId, BusinessSetOperation.Update))
                .ReturnsAsync(new ExistsResult<Order> { Entity = null });

            customerValidationMock.Setup(m => m.Exists(order.CustomerId, BusinessSetOperation.Get))
                .ReturnsAsync(new ExistsResult<Customer> { Entity = CreateCustomer() });

            var result = await sut.InvokeAsync(order, BusinessSetOperation.Update);

            result.ValidationResult.IsValid.Should().BeFalse();
            result.ValidationResult.Errors.Should().Contain("No record found");
        }

        

        [Fact]
        public async Task Should_ReturnError_IfOrderHasNoItems()
        {
            var order = CreateOrder(orderItems: new List<OrderItem>());

            customerValidationMock.Setup(m => m.Exists(order.CustomerId, BusinessSetOperation.Get))
                .ReturnsAsync(new ExistsResult<Customer> { Entity = CreateCustomer() });

            var result = await sut.InvokeAsync(order, BusinessSetOperation.Create);

            result.ValidationResult.Errors.Should().Contain("Order needs to have at least one product");
        }

        

        [Fact]
        public async Task Should_ReturnError_IfSameProductUsedMultipleTimes()
        {
            var order = CreateOrder(new List<OrderItem> 
            {
                CreateOrderItem(1, 123, 1, 1),
                CreateOrderItem(1, 123, 2, 1)
            });

            customerValidationMock.Setup(m => m.Exists(order.CustomerId, BusinessSetOperation.Get))
                .ReturnsAsync(new ExistsResult<Customer> { Entity = CreateCustomer() });

            var result = await sut.InvokeAsync(order, BusinessSetOperation.Create);

            result.ValidationResult.Errors.Should().Contain("Order needs to have at least one product");
        }

        [Fact]
        public async Task Should_ReturnError_IfProductNotFound()
        {
            var order = CreateOrder(new List<OrderItem> 
            {
                CreateOrderItem(1, 0, 1, 1)
            });

            customerValidationMock.Setup(m => m.Exists(order.CustomerId, BusinessSetOperation.Get))
                .ReturnsAsync(new ExistsResult<Customer> { Entity = CreateCustomer() });

            productValidationMock.Setup(m => m.Exists(1, BusinessSetOperation.Get))
                .ReturnsAsync(new ExistsResult<Product> { Entity = null });

            var result = await sut.InvokeAsync(order, BusinessSetOperation.Create);

            result.ValidationResult.Errors.Should().Contain("Order needs to have at least one product");
        }

       

        
        private Order CreateOrder(List<OrderItem>? orderItems = null)
        {
            return new Order(123, 1, DateTime.Now, new TotalAmount(1), new ProductQuantity(1));
        }

        private Customer CreateCustomer()
        {
            return new Customer(1, new FirstName("John"), new LastName("Doe"), new EmailAddress("a@b.com"), new StreetAddress("a"), new PostalCode("a"));
        }
        
        private OrderItem CreateOrderItem(int productId, int orderId, int quatity, decimal price)
        {
            return new OrderItem(1,orderId, new ItemProductQuantity(quatity), new ItemProductPrice(price));
        }








    } 
}
