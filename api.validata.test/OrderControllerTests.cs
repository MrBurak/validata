using api.validata.com.Controllers;
using business.validata.com.Interfaces;
using Microsoft.Extensions.Logging;
using model.validata.com;
using model.validata.com.Entities;
using model.validata.com.Enumeration;
using model.validata.com.Order;
using Moq;


namespace api.validata.test
{


    public class OrderControllerTests
    {
        private readonly Mock<ILogger<OrderController>> _loggerMock = new();
        private readonly Mock<IOrderCommandBusiness> _commandMock=new();
        private readonly Mock<IOrderQueryBusiness> _queryMock = new();
        private readonly OrderController _controller;

        public OrderControllerTests()
        {
            

            _controller = new OrderController(
                _loggerMock.Object,
                _queryMock.Object,
                _commandMock.Object
            );
        }

        [Fact]
        public void Constructor_ThrowsIfLoggerIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new OrderController(
                    null!,
                    _queryMock.Object,
                    _commandMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsIfQueryBusinessIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new OrderController(
                    _loggerMock.Object,
                    null!,
                    _commandMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsIfCommandBusinessIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new OrderController(
                    _loggerMock.Object,
                    _queryMock.Object,
                    null!));
        }

        [Fact]
        public void Constructor_Succeeds_WhenAllDependenciesAreProvided()
        {
            var controller = new OrderController(
                _loggerMock.Object,
                _queryMock.Object,
                _commandMock.Object);

            Assert.NotNull(controller);
        }

        [Fact]
        public async Task List_ReturnsOrders_WhenQuerySucceeds()
        {
            var expected = new QueryResult<IEnumerable<OrderViewModel>>
            {
                Success = true,
                Data = new List<OrderViewModel> { new() }
            };

            _queryMock.Setup(q => q.ListAsync(1, It.IsAny<PaginationRequest>())).ReturnsAsync(expected);

            var result = await _controller.List(1, 1, 10);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task Insert_ReturnsSuccess_WhenCommandSucceeds()
        {
            var insertModel = new OrderInsertModel
            {
                CustomerId = 1,
                Items = new List<OrderItemInsertModel> { new() }
            };

            var expected = new CommandResult<OrderDetailViewModel> { Success = true };

            _commandMock
                .Setup(c => c.InvokeAsync(
                    It.Is<OrderUpdateModel>(o => o.CustomerId == insertModel.CustomerId),
                    BusinessSetOperation.Create))
                .ReturnsAsync(expected);

            var result = await _controller.Insert(insertModel);

            Assert.True(result.Success);
        }

        [Fact]
        public async Task Update_ReturnsSuccess_WhenOrderUpdated()
        {
            var updateModel = new OrderUpdateModel { OrderId = 123 };
            var expected = new CommandResult<OrderDetailViewModel> { Success = true };

            _commandMock
                .Setup(c => c.InvokeAsync(updateModel, BusinessSetOperation.Update))
                .ReturnsAsync(expected);

            var result = await _controller.Update(updateModel);

            Assert.True(result.Success);
        }

        [Fact]
        public async Task Delete_ReturnsSuccess_WhenDeleted()
        {
            var expected = new CommandResult<Order> { Success = true };

            _commandMock.Setup(c => c.DeleteAsync(123)).ReturnsAsync(expected);

            var result = await _controller.Delete(123);

            Assert.True(result.Success);
        }

        [Fact]
        public async Task Get_ReturnsOrder_WhenFound()
        {
            var expected = new QueryResult<OrderDetailViewModel?>
            {
                Success = true,
                Data = new OrderDetailViewModel()
            };

            _queryMock.Setup(q => q.GetAsync(100, 200)).ReturnsAsync(expected);

            var result = await _controller.Get(100, 200);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
        }
    }

}
