using business.validata.com;
using business.validata.com.Interfaces;
using business.validata.com.Interfaces.Utils;
using business.validata.com.Interfaces.Validators;
using business.validata.com.Validators.Models;
using data.validata.com.Entities;
using data.validata.com.Interfaces.Repository;
using model.validata.com.Enumeration;
using model.validata.com.Order;
using model.validata.com.Validators;
using Moq;
using System.Linq.Expressions;
using util.validata.com;

namespace business.validata.test
{
    public class OrderCommandBusinessTests
    {
        private readonly Mock<IOrderValidation> _mockOrderValidation;
        private readonly Mock<ICommandRepository<Order>> _mockOrderRepository;
        private readonly Mock<IGenericValidation<Order>> _mockGenericValidation;
        private readonly Mock<IGenericLambdaExpressions> _mockGenericLambdaExpressions;
        private readonly Mock<IOrderItemCommandBusiness> _mockOrderItemCommandBusiness;
        private readonly Mock<IOrderAdaptor> _mockOrderAdaptor;
        private readonly OrderCommandBusiness _orderCommandBusiness;
        private const float productPrice = 10f;

        public OrderCommandBusinessTests()
        {
            _mockOrderValidation = new Mock<IOrderValidation>();
            _mockOrderRepository = new Mock<ICommandRepository<Order>>();
            _mockGenericValidation = new Mock<IGenericValidation<Order>>();
            _mockGenericLambdaExpressions = new Mock<IGenericLambdaExpressions>();
            _mockOrderItemCommandBusiness = new Mock<IOrderItemCommandBusiness>();
            _mockOrderAdaptor = new Mock<IOrderAdaptor>();

            _orderCommandBusiness = new OrderCommandBusiness(
                _mockOrderValidation.Object,
                _mockOrderRepository.Object,
                _mockGenericValidation.Object,
                _mockGenericLambdaExpressions.Object,
                _mockOrderItemCommandBusiness.Object,
                _mockOrderAdaptor.Object
            );

            

            _mockOrderRepository.Setup(r => r.AddAsync(It.IsAny<Order>()))
                .ReturnsAsync((Order order) => { order.OrderId = order.OrderId > 0 ? order.OrderId : 1; return order; });
            _mockOrderRepository.Setup(r => r.UpdateAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<List<Action<Order>>>()))
                .Returns(Task.CompletedTask);
           
            var order= new Order { OrderId = 1, CustomerId = 10, TotalAmount = 100f, ProductCount = 2, OrderDate = DateTime.UtcNow };

            _mockOrderRepository.Setup(r => r.GetEntityAsync(It.IsAny<Expression<Func<Order, bool>>>()))
                .ReturnsAsync(order);

            _mockGenericLambdaExpressions.Setup(gle => gle.GetEntityByPrimaryKey(It.IsAny<Order>()))
                .Returns<Order>(order => o => o.OrderId == order.OrderId);
            _mockGenericLambdaExpressions.Setup(gle => gle.GetEntityById<Order>(It.IsAny<int>()))
                .Returns<int>(id => o => o.OrderId == id);


            _mockOrderAdaptor.Setup(a => a.Invoke(It.IsAny<OrderUpdateModel>(), It.IsAny<BusinessSetOperation>()))
                .ReturnsAsync((OrderUpdateModel model, BusinessSetOperation op) =>
                {
                    var order = new Order
                    {
                        OrderId = model.OrderId,
                        CustomerId = model.CustomerId,
                        OrderItems = model.Items.Select(item => new OrderItem
                        {
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            ProductPrice = productPrice
                        }).ToList(),
                        ProductCount = model.Items.Sum(x => x.Quantity),
                        TotalAmount = model.Items.Sum(x => x.Quantity * productPrice),
                    };
                    return order;
                });
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfValidationIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new OrderCommandBusiness(
                null!, _mockOrderRepository.Object, _mockGenericValidation.Object,
                _mockGenericLambdaExpressions.Object, _mockOrderItemCommandBusiness.Object, _mockOrderAdaptor.Object));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfGenericLambdaExpressionsIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new OrderCommandBusiness(
                _mockOrderValidation.Object, _mockOrderRepository.Object, _mockGenericValidation.Object,
                null!, _mockOrderItemCommandBusiness.Object, _mockOrderAdaptor.Object));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfOrderAdaptorIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new OrderCommandBusiness(
                _mockOrderValidation.Object, _mockOrderRepository.Object, _mockGenericValidation.Object,
                _mockGenericLambdaExpressions.Object, _mockOrderItemCommandBusiness.Object, null!));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfOrderItemCommandBusinessIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new OrderCommandBusiness(
                _mockOrderValidation.Object, 
                _mockOrderRepository.Object, _mockGenericValidation.Object,
                _mockGenericLambdaExpressions.Object, null!, _mockOrderAdaptor.Object));
        }

        [Fact]
        public async Task InvokeAsync_ValidationFails_ReturnsValidationErrors()
        {
            var err = new ValidationResult<Order>();
            err.AddError("Invalid order");
            var orderUpdateModel = new OrderUpdateModel { OrderId = 0, CustomerId = 1, Items = new List<OrderItemInsertModel>() };
            var order = new Order { OrderId = 0, CustomerId = 1, OrderItems = new List<OrderItem>() };
            var validationResult = new OrderValidationResult
            {
                ValidationResult = err
            };

            _mockOrderAdaptor.Setup(a => a.Invoke(orderUpdateModel, BusinessSetOperation.Create)).ReturnsAsync(order);
            _mockOrderValidation.Setup(v => v.InvokeAsync(order, BusinessSetOperation.Create)).ReturnsAsync(validationResult);

            var result = await _orderCommandBusiness.InvokeAsync(orderUpdateModel, BusinessSetOperation.Create);

            Assert.False(result.Success);
            Assert.Contains("Invalid order", result.Validations);
            Assert.Null(result.Result);
            _mockOrderAdaptor.Verify(a => a.Invoke(orderUpdateModel, BusinessSetOperation.Create), Times.Once);
            _mockOrderValidation.Verify(v => v.InvokeAsync(order, BusinessSetOperation.Create), Times.Once);
            _mockOrderRepository.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Never);
            _mockOrderItemCommandBusiness.Verify(oicb => oicb.AddAsync(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task InvokeAsync_CreateOperation_SucceedsAndAddsOrderItems()
        {
            var orderUpdateModel = new OrderUpdateModel
            {
                OrderId = 0,
                CustomerId = 1,
                Items = new List<OrderItemInsertModel>
                {
                    new OrderItemInsertModel { ProductId = 1, Quantity = 2, }
                }
            };
            var order = new Order
            {
                OrderId = 0,
                CustomerId = 1,
                OrderItems = orderUpdateModel.Items.Select(i => new OrderItem
                { ProductId = i.ProductId, Quantity = i.Quantity, ProductPrice = productPrice }).ToList(),
                ProductCount = 2,
                TotalAmount = productPrice * 2
            };
            var validatedOrder = new Order
            {
                OrderId = 0,
                CustomerId = 1,
                OrderItems = order.OrderItems,
                ProductCount = order.ProductCount,
                TotalAmount = order.TotalAmount
            };
            var products = new List<Product> { new Product { ProductId = 1, Name = "Test Product" } };

            _mockOrderAdaptor.Setup(a => a.Invoke(orderUpdateModel, BusinessSetOperation.Create)).ReturnsAsync(order);
            _mockOrderValidation.Setup(v => v.InvokeAsync(order, BusinessSetOperation.Create))
                .ReturnsAsync(new OrderValidationResult
                {
                    ValidationResult = new ValidationResult<Order> { Entity = validatedOrder },
                    Products = products
                });
            _mockOrderRepository.Setup(r => r.AddAsync(validatedOrder)).ReturnsAsync(new Order { OrderId = 1, CustomerId = 1, OrderDate = DateTimeUtil.SystemTime, ProductCount = 2, TotalAmount = 20.00f });
            _mockOrderItemCommandBusiness.Setup(oicb => oicb.AddAsync(It.IsAny<Order>()))
                .ReturnsAsync(new List<OrderItem>
                {
                new OrderItem { OrderId = 1, ProductId = 1, Quantity = 2, ProductPrice = 10.00f }
                });

            var result = await _orderCommandBusiness.InvokeAsync(orderUpdateModel, BusinessSetOperation.Create);

            Assert.True(result.Success);
            Assert.Empty(result.Validations);
            Assert.NotNull(result.Result);
            Assert.Equal(1, result.Result!.OrderId);
            Assert.Equal(2, result.Result.ProductCount);
            Assert.Equal(20.00f, result.Result.TotalAmount);
            Assert.Single(result.Result.Items);
            Assert.Equal("Test Product", result.Result.Items.First().ProductName);
         
        }

        [Fact]
        public async Task InvokeAsync_UpdateOperation_SucceedsAndAddsOrderItems()
        {
            var orderUpdateModel = new OrderUpdateModel
            {
                OrderId = 1,
                CustomerId = 1,
                Items = new List<OrderItemInsertModel>
            {
                new OrderItemInsertModel { ProductId = 1, Quantity = 3 }
            }
            };
            var order = new Order
            {
                OrderId = 1,
                CustomerId = 1,
                OrderItems = orderUpdateModel.Items.Select(i => new OrderItem
                { ProductId = i.ProductId, Quantity = i.Quantity, ProductPrice = productPrice }).ToList(),
                ProductCount = 3,
                TotalAmount = 30.00f
            };
            var validatedOrder = new Order
            {
                OrderId = 1,
                CustomerId = 1,
                OrderItems = order.OrderItems,
                ProductCount = order.ProductCount,
                TotalAmount = order.TotalAmount
            };
            var products = new List<Product> { new Product { ProductId = 1, Name = "Updated Product" } };

            _mockOrderAdaptor.Setup(a => a.Invoke(orderUpdateModel, BusinessSetOperation.Update)).ReturnsAsync(order);
            _mockOrderValidation.Setup(v => v.InvokeAsync(order, BusinessSetOperation.Update))
                .ReturnsAsync(new OrderValidationResult
                {
                    ValidationResult = new ValidationResult<Order> { Entity = validatedOrder },
                    Products = products
                });
            _mockOrderRepository.Setup(r => r.GetEntityAsync(It.IsAny<Expression<Func<Order, bool>>>()))
                .ReturnsAsync(new Order { OrderId = 1, CustomerId = 1, OrderDate = DateTimeUtil.SystemTime, ProductCount = 3, TotalAmount = 36.00f });
            _mockOrderItemCommandBusiness.Setup(oicb => oicb.AddAsync(It.IsAny<Order>()))
                .ReturnsAsync(new List<OrderItem>
                {
                new OrderItem { OrderId = 1, ProductId = 1, Quantity = 3, ProductPrice = 12.00f }
                });

            var result = await _orderCommandBusiness.InvokeAsync(orderUpdateModel, BusinessSetOperation.Update);

            Assert.True(result.Success);
            Assert.Empty(result.Validations);
            Assert.NotNull(result.Result);
            Assert.Equal(1, result.Result!.OrderId);
            Assert.Equal(3, result.Result.ProductCount);
            Assert.Equal(36.00f, result.Result.TotalAmount);
            Assert.Single(result.Result.Items);
            Assert.Equal("Updated Product", result.Result.Items.First().ProductName);
            _mockOrderAdaptor.Verify(a => a.Invoke(orderUpdateModel, BusinessSetOperation.Update), Times.Once);
            _mockOrderValidation.Verify(v => v.InvokeAsync(order, BusinessSetOperation.Update), Times.Once);
            _mockOrderRepository.Verify(r => r.UpdateAsync(
                It.Is<Expression<Func<Order, bool>>>(exp => exp.Compile().Invoke(validatedOrder)),
                It.IsAny<List<Action<Order>>>()), Times.Once);
            _mockOrderRepository.Verify(r => r.GetEntityAsync(It.IsAny<Expression<Func<Order, bool>>>()), Times.Once);
            _mockOrderItemCommandBusiness.Verify(oicb => oicb.AddAsync(It.Is<Order>(o => o.OrderId == 1)), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_HandlesException_ReturnsFailure()
        {
            var orderUpdateModel = new OrderUpdateModel
            {
                OrderId = 1,
                CustomerId = 1,
                Items = new List<OrderItemInsertModel>()
            };
            var order = new Order { OrderId = 1, CustomerId = 1, OrderItems = new List<OrderItem>() };
            var validatedOrder = new Order { OrderId = 1, CustomerId = 1 };
            var expectedExceptionMessage = "Something went wrong in the repository";

            _mockOrderAdaptor.Setup(a => a.Invoke(orderUpdateModel, BusinessSetOperation.Update)).ReturnsAsync(order);
            _mockOrderValidation.Setup(v => v.InvokeAsync(order, BusinessSetOperation.Update))
                .ReturnsAsync(new OrderValidationResult
                {
                    ValidationResult = new ValidationResult<Order> { Entity = validatedOrder }
                });
            _mockOrderRepository.Setup(r => r.UpdateAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<List<Action<Order>>>()))
                .ThrowsAsync(new Exception(expectedExceptionMessage));

            var result = await _orderCommandBusiness.InvokeAsync(orderUpdateModel, BusinessSetOperation.Update);

            Assert.False(result.Success);
            Assert.Equal(expectedExceptionMessage, result.Exception);
            Assert.Null(result.Result);
            _mockOrderRepository.Verify(r => r.UpdateAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<List<Action<Order>>>()), Times.Once);
            _mockOrderItemCommandBusiness.Verify(oicb => oicb.AddAsync(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_DeletesOrderAndOrderItemsSuccessfully()
        {
            int orderIdToDelete = 1;
            var orderToDelete = new Order { OrderId = orderIdToDelete };

            _mockGenericValidation.Setup(gv => gv.Exists(orderIdToDelete, BusinessSetOperation.Delete))
                .ReturnsAsync(new ExistsResult<Order> { Entity = orderToDelete });
            _mockGenericLambdaExpressions.Setup(gle => gle.GetEntityById<Order>(orderIdToDelete))
                .Returns<int>(id => o => o.OrderId == id);
            _mockOrderRepository.Setup(r => r.UpdateAsync(
                It.IsAny<Expression<Func<Order, bool>>>(),
                It.IsAny<List<Action<Order>>>()))
                .Returns(Task.CompletedTask);
            _mockOrderItemCommandBusiness.Setup(oicb => oicb.DeleteAllAsync(orderIdToDelete))
                .Returns(Task.CompletedTask);

            var result = await _orderCommandBusiness.DeleteAsync(orderIdToDelete);

            Assert.True(result.Success);
            Assert.Empty(result.Validations);
            _mockGenericValidation.Verify(gv => gv.Exists(orderIdToDelete, BusinessSetOperation.Delete), Times.Once);
            _mockOrderRepository.Verify(r => r.UpdateAsync(
                It.Is<Expression<Func<Order, bool>>>(exp => exp.Compile().Invoke(orderToDelete)),
                It.IsAny<List<Action<Order>>>()), Times.Once);
            _mockOrderItemCommandBusiness.Verify(oicb => oicb.DeleteAllAsync(orderIdToDelete), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_HandlesBaseDeleteFailure()
        {
            int orderIdToDelete = 1;

            _mockGenericValidation.Setup(gv => gv.Exists(orderIdToDelete, BusinessSetOperation.Delete))
                .ReturnsAsync(new ExistsResult<Order> { Entity = null, }); 
            var result = await _orderCommandBusiness.DeleteAsync(orderIdToDelete);

            Assert.False(result.Success);
            Assert.Single(result.Validations);
            Assert.Equal("No record found", result.Validations.First());
            _mockGenericValidation.Verify(gv => gv.Exists(orderIdToDelete, BusinessSetOperation.Delete), Times.Once);
            _mockOrderRepository.Verify(r => r.UpdateAsync(
                It.IsAny<Expression<Func<Order, bool>>>(),
                It.IsAny<List<Action<Order>>>()), Times.Never);
            _mockOrderItemCommandBusiness.Verify(oicb => oicb.DeleteAllAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAllAsync_DeletesOrdersAndOrderItemsForCustomer()
        {
            int customerId = 5;
            var ordersToDelete = new List<Order>
        {
            new Order { OrderId = 1, CustomerId = customerId },
            new Order { OrderId = 2, CustomerId = customerId }
        };

            _mockOrderRepository.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<Order, bool>>>()))
                .ReturnsAsync(ordersToDelete);
            _mockOrderRepository.Setup(r => r.UpdateAsync(
                It.IsAny<Expression<Func<Order, bool>>>(),
                It.IsAny<List<Action<Order>>>()))
                .Returns(Task.CompletedTask);
            _mockOrderItemCommandBusiness.Setup(oicb => oicb.DeleteAllForCustomerAsync(customerId))
                .Returns(Task.CompletedTask);

            await _orderCommandBusiness.DeleteAllAsync(customerId);

            _mockOrderRepository.Verify(r => r.UpdateAsync(
                It.Is<Expression<Func<Order, bool>>>(exp => exp.Compile().Invoke(new Order { CustomerId = customerId, DeletedOn = null })),
                It.IsAny<List<Action<Order>>>()), Times.Once);
            _mockOrderItemCommandBusiness.Verify(oicb => oicb.DeleteAllForCustomerAsync(customerId), Times.Once);
        }
    }
}
