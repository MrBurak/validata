using business.validata.com;
using business.validata.com.Interfaces.Utils;
using business.validata.com.Interfaces.Validators;
using data.validata.com.Entities;
using data.validata.com.Interfaces.Repository;
using Moq;
using System.Linq.Expressions;

namespace business.validata.test
{
    public class OrderItemCommandBusinessTests
    {
        private readonly Mock<ICommandRepository<OrderItem>> _mockOrderItemRepository;
        private readonly Mock<ICommandRepository<Order>> _mockOrderRepository;
        private readonly Mock<IGenericValidation<OrderItem>> _mockGenericValidation;
        private readonly Mock<IGenericLambdaExpressions> _mockGenericLambdaExpressions;
        private readonly OrderItemCommandBusiness _orderItemCommandBusiness;

        public OrderItemCommandBusinessTests()
        {
            _mockOrderItemRepository = new Mock<ICommandRepository<OrderItem>>();
            _mockOrderRepository = new Mock<ICommandRepository<Order>>();
            _mockGenericValidation = new Mock<IGenericValidation<OrderItem>>();
            _mockGenericLambdaExpressions = new Mock<IGenericLambdaExpressions>();

            _orderItemCommandBusiness = new OrderItemCommandBusiness(
                _mockOrderItemRepository.Object,
                _mockOrderRepository.Object,
                _mockGenericValidation.Object,
                _mockGenericLambdaExpressions.Object
            );

           
            _mockOrderItemRepository.Setup(r => r.UpdateAsync(
                It.IsAny<Expression<Func<OrderItem, bool>>>(),
                It.IsAny<List<Action<OrderItem>>>()))
                .Returns(Task.CompletedTask);

            _mockOrderRepository.Setup(r => r.UpdateAsync(
                It.IsAny<Expression<Func<Order, bool>>>(),
                It.IsAny<List<Action<Order>>>()))
                .Returns(Task.CompletedTask);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfOrderItemRepositoryIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new OrderItemCommandBusiness(
                null!, _mockOrderRepository.Object, _mockGenericValidation.Object, _mockGenericLambdaExpressions.Object));
            Assert.Equal("repository", ex.ParamName);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfOrderRepositoryIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new OrderItemCommandBusiness(
                _mockOrderItemRepository.Object, null!, _mockGenericValidation.Object, _mockGenericLambdaExpressions.Object));
           
            Assert.Equal("repositoryOrder", ex.ParamName);
        }

        [Fact]
        public async Task AddAsync_DeletesExistingItemsAndAddsNewOnes()
        {
            var order = new Order
            {
                OrderId = 1,
                OrderItems = new List<OrderItem>
            {
                new OrderItem { ProductId = 101, Quantity = 2, ProductPrice = 10.00f, OperationSourceId = 1 },
                new OrderItem { ProductId = 102, Quantity = 1, ProductPrice = 20.00f, OperationSourceId = 1 }
            }
            };

           
            _mockOrderItemRepository.Setup(r => r.AddAsync(It.IsAny<OrderItem>()))
                .ReturnsAsync((OrderItem item) =>
                {
                    item.OrderId = new Random().Next(1, 1000); 
                    return item;
                });

            var result = await _orderItemCommandBusiness.AddAsync(order);

            _mockOrderItemRepository.Verify(r => r.UpdateAsync(
                It.Is<Expression<Func<OrderItem, bool>>>(expr => expr.Compile().Invoke(new OrderItem { OrderId = order.OrderId, DeletedOn = null })),
                It.IsAny<List<Action<OrderItem>>>()), Times.Once);

            _mockOrderItemRepository.Verify(r => r.AddAsync(It.IsAny<OrderItem>()), Times.Exactly(order.OrderItems.Count));

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, item => Assert.True(item.OrderId > 0)); 

        }

        [Fact]
        public async Task AddAsync_HandlesEmptyOrderItemsList()
        {
            var order = new Order
            {
                OrderId = 1,
                OrderItems = new List<OrderItem>()
            };

            var result = await _orderItemCommandBusiness.AddAsync(order);

            _mockOrderItemRepository.Verify(r => r.UpdateAsync(
                It.Is<Expression<Func<OrderItem, bool>>>(expr => expr.Compile().Invoke(new OrderItem { OrderId = order.OrderId, DeletedOn = null })),
                It.IsAny<List<Action<OrderItem>>>()), Times.Once);

            _mockOrderItemRepository.Verify(r => r.AddAsync(It.IsAny<OrderItem>()), Times.Never);

            Assert.NotNull(result);
            Assert.Empty(result);
        }


        [Fact]
        public async Task DeleteAllAsync_CallsBaseDeleteAllAsyncWithCorrectExpression()
        {
            int orderIdToDelete = 5;

            await _orderItemCommandBusiness.DeleteAllAsync(orderIdToDelete);

            _mockOrderItemRepository.Verify(r => r.UpdateAsync(
                It.Is<Expression<Func<OrderItem, bool>>>(expr => expr.Compile().Invoke(new OrderItem { OrderId = orderIdToDelete, DeletedOn = null })),
                It.IsAny<List<Action<OrderItem>>>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAllForCustomerAsync_DeletesOrderItemsForCustomerOrders()
        {
            int customerId = 10;
            var ordersForCustomer = new List<Order>
        {
            new Order { OrderId = 101, CustomerId = customerId },
            new Order { OrderId = 102, CustomerId = customerId }
        };

            _mockOrderRepository.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<Order, bool>>>()))
                .ReturnsAsync(ordersForCustomer);

            await _orderItemCommandBusiness.DeleteAllForCustomerAsync(customerId);
_mockOrderRepository.Verify(r => r.GetListAsync(
                It.Is<Expression<Func<Order, bool>>>(expr => expr.Compile().Invoke(new Order { CustomerId = customerId, DeletedOn = null }))), Times.Once);

            _mockOrderItemRepository.Verify(r => r.UpdateAsync(
                It.Is<Expression<Func<OrderItem, bool>>>(expr => expr.Compile().Invoke(new OrderItem { OrderId = 101, DeletedOn = null }) &&
                                                                           expr.Compile().Invoke(new OrderItem { OrderId = 102, DeletedOn = null })),
                It.IsAny<List<Action<OrderItem>>>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAllForCustomerAsync_DoesNothingIfNoOrdersForCustomer()
        {
            int customerId = 20;
            var noOrders = new List<Order>();

            _mockOrderRepository.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<Order, bool>>>()))
                .ReturnsAsync(noOrders);

            await _orderItemCommandBusiness.DeleteAllForCustomerAsync(customerId);

            _mockOrderRepository.Verify(r => r.GetListAsync(It.IsAny<Expression<Func<Order, bool>>>()), Times.Once);

            _mockOrderItemRepository.Verify(r => r.UpdateAsync(
                It.IsAny<Expression<Func<OrderItem, bool>>>(),
                It.IsAny<List<Action<OrderItem>>>()), Times.Once);
        }
}




}
