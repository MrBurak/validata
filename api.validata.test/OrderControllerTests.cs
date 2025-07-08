using api.validata.com.Controllers;
using business.validata.com.Interfaces;
using data.validata.com.Entities;
using Microsoft.Extensions.Logging;
using model.validata.com;
using model.validata.com.Enumeration;
using model.validata.com.Order;
using Moq;


namespace api.validata.test
{
    public class OrderControllerTests
    {
        private readonly Mock<ILogger<OrderController>> _mockLogger;
        private readonly Mock<IOrderCommandBusiness> _mockCommandBusiness;
        private readonly Mock<IOrderQueryBusiness> _mockQueryBusiness;
        private readonly OrderController _controller;

        public OrderControllerTests()
        {
            _mockLogger = new Mock<ILogger<OrderController>>();
            _mockCommandBusiness = new Mock<IOrderCommandBusiness>();
            _mockQueryBusiness = new Mock<IOrderQueryBusiness>();
            _controller = new OrderController(
                _mockLogger.Object,
                _mockQueryBusiness.Object,
                _mockCommandBusiness.Object
            );
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfLoggerIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new OrderController(
                null!, _mockQueryBusiness.Object, _mockCommandBusiness.Object));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfCommandBusinessIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new OrderController(
                _mockLogger.Object, _mockQueryBusiness.Object, null!));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfQueryBusinessIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new OrderController(
                _mockLogger.Object, null!, _mockCommandBusiness.Object));
        }

        [Fact]
        public async Task List_ReturnsQueryResultFromQueryBusiness()
        {
            int customerId = 123;
            var expectedResult = new QueryResult<IEnumerable<OrderViewModel>>
            {
                Success = true,
                Result = new List<OrderViewModel>
            {
                new OrderViewModel { OrderId = 1, TotalAmount = 100.00f },
                new OrderViewModel { OrderId = 2, TotalAmount = 250.00f }
            }
            };

            _mockQueryBusiness.Setup(b => b.ListAsync(customerId)).ReturnsAsync(expectedResult);

            var result = await _controller.List(customerId);

            Assert.Equal(expectedResult, result);
            _mockQueryBusiness.Verify(b => b.ListAsync(customerId), Times.Once);
        }

        [Fact]
        public async Task Insert_MapsRequestAndInvokesCommandBusiness()
        {
            var insertModel = new OrderInsertModel
            {
                CustomerId = 1,
                Items = new List<OrderItemInsertModel> { new OrderItemInsertModel { ProductId = 101, Quantity = 1 } }
            };
            var expectedOrderDetail = new OrderDetailViewModel
            {
                OrderId = 1,
                TotalAmount = 50.00f,
                Items = new List<OrderItemViewModel>()
            };
            var expectedCommandResult = new CommandResult<OrderDetailViewModel>
            {
                Success = true,
                Result = expectedOrderDetail
            };

            _mockCommandBusiness.Setup(b => b.InvokeAsync(
                    It.IsAny<OrderUpdateModel>(),
                    BusinessSetOperation.Create))
                .ReturnsAsync(expectedCommandResult);

            var result = await _controller.Insert(insertModel);

            Assert.Equal(expectedCommandResult, result);
            _mockCommandBusiness.Verify(b => b.InvokeAsync(
                It.Is<OrderUpdateModel>(o => o.CustomerId == insertModel.CustomerId),
                BusinessSetOperation.Create), Times.Once);
        }

        [Fact]
        public async Task Update_InvokesCommandBusinessWithRequest()
        {
            var updateModel = new OrderUpdateModel
            {
                OrderId = 1,
                CustomerId = 1,
                Items = new List<OrderItemInsertModel> { new OrderItemInsertModel { ProductId = 101, Quantity = 2} }
            };
            var expectedOrderDetail = new OrderDetailViewModel
            {
                OrderId = 1,
                Items = new List<OrderItemViewModel>()
            };
            var expectedCommandResult = new CommandResult<OrderDetailViewModel>
            {
                Success = true,
                Result = expectedOrderDetail
            };

            _mockCommandBusiness.Setup(b => b.InvokeAsync(updateModel, BusinessSetOperation.Update))
                .ReturnsAsync(expectedCommandResult);

            var result = await _controller.Update(updateModel);

            Assert.Equal(expectedCommandResult, result);
            _mockCommandBusiness.Verify(b => b.InvokeAsync(updateModel, BusinessSetOperation.Update), Times.Once);
        }

        [Fact]
        public async Task Delete_InvokesCommandBusinessDelete()
        {
            int orderIdToDelete = 1;
            var expectedCommandResult = new CommandResult<Order>
            {
                Success = true,
                Result = new Order { OrderId = orderIdToDelete }
            };

            _mockCommandBusiness.Setup(b => b.DeleteAsync(orderIdToDelete)).ReturnsAsync(expectedCommandResult);

            var result = await _controller.Delete(orderIdToDelete);

            Assert.Equal(expectedCommandResult, result);
            _mockCommandBusiness.Verify(b => b.DeleteAsync(orderIdToDelete), Times.Once);
        }

        [Fact]
        public async Task Get_ReturnsQueryResultFromQueryBusiness()
        {
            int orderId = 1;
            int customerId = 123;
            var expectedResult = new QueryResult<OrderDetailViewModel?>
            {
                Success = true,
                Result = new OrderDetailViewModel { OrderId = orderId, TotalAmount = 150.00f }
            };

            _mockQueryBusiness.Setup(b => b.GetAsync(orderId, customerId)).ReturnsAsync(expectedResult);

            var result = await _controller.Get(orderId, customerId);

            Assert.Equal(expectedResult, result);
            _mockQueryBusiness.Verify(b => b.GetAsync(orderId, customerId), Times.Once);
        }
    }
}
