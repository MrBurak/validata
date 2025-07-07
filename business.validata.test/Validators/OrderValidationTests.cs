using business.validata.com.Interfaces.Validators;
using business.validata.com.Validators;
using data.validata.com.Entities;
using data.validata.com.Interfaces.Repository;
using model.validata.com.Enumeration;
using model.validata.com.Validators;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.validata.test.Validators
{
    public class OrderValidationTests
    {
        private readonly Mock<IGenericValidation<Customer>> _mockGenericValidationCustomer;
        private readonly Mock<IGenericValidation<Product>> _mockGenericValidationProduct;
        private readonly Mock<IGenericValidation<Order>> _mockGenericValidationOrder;
        private readonly OrderValidation _orderValidation;

        public OrderValidationTests()
        {
            _mockGenericValidationCustomer = new Mock<IGenericValidation<Customer>>();
            _mockGenericValidationProduct = new Mock<IGenericValidation<Product>>();
            _mockGenericValidationOrder = new Mock<IGenericValidation<Order>>();

            _orderValidation = new OrderValidation(
                _mockGenericValidationCustomer.Object,
                _mockGenericValidationProduct.Object,
                _mockGenericValidationOrder.Object
            );

            _mockGenericValidationOrder.Setup(m => m.Exists(It.IsAny<int>(), BusinessSetOperation.Update))
                .ReturnsAsync(new ExistsResult<Order> { Entity = new Order { OrderId = 1, CustomerId = 10 } });
            _mockGenericValidationOrder.Setup(m => m.Exists(It.IsAny<int>(), BusinessSetOperation.Get))
                .ReturnsAsync(new ExistsResult<Order> { Entity = new Order { OrderId = 1, CustomerId = 10 } });
            _mockGenericValidationOrder.Setup(m => m.Exists(It.IsAny<int>(), BusinessSetOperation.Delete))
                .ReturnsAsync(new ExistsResult<Order> { Entity = new Order { OrderId = 1, CustomerId = 10 } });
            _mockGenericValidationOrder.Setup(m => m.Exists(It.IsAny<int>(), BusinessSetOperation.Create))
                .ReturnsAsync((ExistsResult<Order>?)null);

            _mockGenericValidationCustomer.Setup(m => m.Exists(It.IsAny<int>(), BusinessSetOperation.Get))
                .ReturnsAsync(new ExistsResult<Customer> { Entity = new Customer { CustomerId = 10, FirstName = "Test Customer" } });

            _mockGenericValidationProduct.Setup(m => m.Exists(It.IsAny<int>(), BusinessSetOperation.Get))
                .ReturnsAsync(new ExistsResult<Product> { Entity = new Product { ProductId = 101, Name = "Test Product", Price = 10.0f } });
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfGenericValidationCustomerIsNull()
        {
            IGenericValidation<Customer> nullCustomerValidation = null!;

            Assert.Throws<ArgumentNullException>(() => new OrderValidation(
                nullCustomerValidation, _mockGenericValidationProduct.Object, _mockGenericValidationOrder.Object));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfGenericValidationProductIsNull()
        {
            IGenericValidation<Product> nullProductValidation = null!;

            Assert.Throws<ArgumentNullException>(() => new OrderValidation(
                _mockGenericValidationCustomer.Object, nullProductValidation, _mockGenericValidationOrder.Object));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfGenericValidationOrderIsNull()
        {
            IGenericValidation<Order> nullOrderValidation = null!;

            Assert.Throws<ArgumentNullException>(() => new OrderValidation(
                _mockGenericValidationCustomer.Object, _mockGenericValidationProduct.Object, nullOrderValidation));
        }

        [Theory]
        [InlineData(BusinessSetOperation.Update)]
        [InlineData(BusinessSetOperation.Delete)]
        [InlineData(BusinessSetOperation.Get)]
        public async Task InvokeAsync_OrderDoesNotExist_ReturnsErrorAndEarlyExits(BusinessSetOperation operation)
        {
            var order = new Order { OrderId = 1, CustomerId = 10 };
            string expectedErrorCode = "No record found";
            _mockGenericValidationOrder.Setup(m => m.Exists(order.OrderId, operation))
                .ReturnsAsync(new ExistsResult<Order> { Entity = null });

            var result = await _orderValidation.InvokeAsync(order, operation);

            Assert.False(result.ValidationResult.IsValid);
            Assert.Contains(expectedErrorCode, result.ValidationResult.Errors);
            Assert.Null(result.ValidationResult.Entity);
            _mockGenericValidationCustomer.Verify(m => m.Exists(It.IsAny<int>(), It.IsAny<BusinessSetOperation>()), Times.Never);
            _mockGenericValidationProduct.Verify(m => m.Exists(It.IsAny<int>(), It.IsAny<BusinessSetOperation>()), Times.Never);
        }

        [Theory]
        [InlineData(BusinessSetOperation.Update)]
        [InlineData(BusinessSetOperation.Delete)]
        [InlineData(BusinessSetOperation.Get)]
        public async Task InvokeAsync_OrderExists_SetsEntityInValidationResult(BusinessSetOperation operation)
        {
            var existingOrder = new Order { OrderId = 1, CustomerId = 10, OrderDate = DateTime.Now };
            var order = new Order { OrderId = 1, CustomerId = 10 };
            _mockGenericValidationOrder.Setup(m => m.Exists(order.OrderId, operation))
                .ReturnsAsync(new ExistsResult<Order> { Entity = existingOrder });

            var result = await _orderValidation.InvokeAsync(order, operation);

            Assert.Same(existingOrder, result.ValidationResult.Entity);
        }

        [Fact]
        public async Task InvokeAsync_OrderExistsReturnsNullForCreate_ReturnsEmptyValidationResult()
        {
            var order = new Order { OrderId = 0, CustomerId = 10 };
            _mockGenericValidationOrder.Setup(m => m.Exists(order.OrderId, BusinessSetOperation.Create))
                .ReturnsAsync((ExistsResult<Order>?)null);

            var result = await _orderValidation.InvokeAsync(order, BusinessSetOperation.Create);

            Assert.True(result.ValidationResult.IsValid);
            Assert.Empty(result.ValidationResult.Errors);
            Assert.Null(result.ValidationResult.Entity);
            _mockGenericValidationCustomer.Verify(m => m.Exists(It.IsAny<int>(), It.IsAny<BusinessSetOperation>()), Times.Never);
            _mockGenericValidationProduct.Verify(m => m.Exists(It.IsAny<int>(), It.IsAny<BusinessSetOperation>()), Times.Never);
        }


        [Fact]
        public async Task InvokeAsync_CustomerDoesNotExist_ReturnsErrorAndEarlyExits()
        {
            var order = new Order { OrderId = 1, CustomerId = 99 };
            _mockGenericValidationCustomer.Setup(m => m.Exists(order.CustomerId, BusinessSetOperation.Get))
                .ReturnsAsync(new ExistsResult<Customer> { Entity = null });

            var result = await _orderValidation.InvokeAsync(order, BusinessSetOperation.Update);

            Assert.False(result.ValidationResult.IsValid);
            Assert.Contains("Customer Not Found", result.ValidationResult.Errors);
            _mockGenericValidationProduct.Verify(m => m.Exists(It.IsAny<int>(), It.IsAny<BusinessSetOperation>()), Times.Never);
            Assert.Empty(result.Products);
        }

        [Fact]
        public async Task InvokeAsync_ExistingOrderBelongsToDifferentCustomer_ReturnsErrorAndEarlyExits()
        {
            var order = new Order { OrderId = 1, CustomerId = 10, OrderItems = { new OrderItem { ProductId = 101, Quantity = 1 } } };
            var existingOrder = new Order { OrderId = 1, CustomerId = 20 };
            var foundCustomer = new Customer { CustomerId = 10 };

            _mockGenericValidationOrder.Setup(m => m.Exists(order.OrderId, BusinessSetOperation.Update))
                .ReturnsAsync(new ExistsResult<Order> { Entity = existingOrder });
            _mockGenericValidationCustomer.Setup(m => m.Exists(order.CustomerId, BusinessSetOperation.Get))
                .ReturnsAsync(new ExistsResult<Customer> { Entity = foundCustomer });

            var result = await _orderValidation.InvokeAsync(order, BusinessSetOperation.Update);

            Assert.False(result.ValidationResult.IsValid);
            Assert.Contains("Order belongs to another customer", result.ValidationResult.Errors);
            Assert.Same(existingOrder, result.ValidationResult.Entity);
            _mockGenericValidationProduct.Verify(m => m.Exists(It.IsAny<int>(), It.IsAny<BusinessSetOperation>()), Times.Never);
            Assert.Empty(result.Products);
        }

        [Fact]
        public async Task InvokeAsync_NoOrderItems_ReturnsError()
        {
            var order = new Order { OrderId = 1, CustomerId = 10, OrderItems = new List<OrderItem>() };

            var result = await _orderValidation.InvokeAsync(order, BusinessSetOperation.Update);

            Assert.False(result.ValidationResult.IsValid);
            Assert.Contains("Order needs to have at least one product", result.ValidationResult.Errors);
            _mockGenericValidationProduct.Verify(m => m.Exists(It.IsAny<int>(), It.IsAny<BusinessSetOperation>()), Times.Never);
            Assert.Empty(result.Products);
        }

        [Fact]
        public async Task InvokeAsync_OrderItemWithZeroQuantity_AddsErrorAndContinuesProcessing()
        {
            var order = new Order
            {
                OrderId = 1,
                CustomerId = 10,
                OrderItems = new List<OrderItem>
            {
                new OrderItem { ProductId = 101, Quantity = 0 },
                new OrderItem { ProductId = 102, Quantity = 5 }
            }
            };

            var product102 = new Product { ProductId = 102, Name = "Product B" };
            _mockGenericValidationProduct.Setup(m => m.Exists(101, BusinessSetOperation.Get))
                .ReturnsAsync(new ExistsResult<Product> { Entity = new Product { ProductId = 101 } }); // Product 101 exists but quantity is 0
            _mockGenericValidationProduct.Setup(m => m.Exists(102, BusinessSetOperation.Get))
                .ReturnsAsync(new ExistsResult<Product> { Entity = product102 });

            var result = await _orderValidation.InvokeAsync(order, BusinessSetOperation.Update);

            Assert.False(result.ValidationResult.IsValid);
            Assert.Contains("Item quantity cannot be zero", result.ValidationResult.Errors);
            _mockGenericValidationProduct.Verify(m => m.Exists(102, BusinessSetOperation.Get), Times.Once);
            Assert.Single(result.Products);
            Assert.Equal(102, result.Products.First().ProductId);
        }

        [Fact]
        public async Task InvokeAsync_SameProductInMultiOrderItem_AddsErrorAndContinuesProcessing()
        {
            var order = new Order
            {
                OrderId = 1,
                CustomerId = 10,
                OrderItems = new List<OrderItem>
            {
                new OrderItem { ProductId = 101, Quantity = 1 },
                new OrderItem { ProductId = 102, Quantity = 2 },
                new OrderItem { ProductId = 101, Quantity = 3 }
            }
            };
            var product101 = new Product { ProductId = 101, Name = "Prod A" };
            var product102 = new Product { ProductId = 102, Name = "Prod B" };

            _mockGenericValidationProduct.Setup(m => m.Exists(101, BusinessSetOperation.Get))
                .ReturnsAsync(new ExistsResult<Product> { Entity = product101 });
            _mockGenericValidationProduct.Setup(m => m.Exists(102, BusinessSetOperation.Get))
                .ReturnsAsync(new ExistsResult<Product> { Entity = product102 });

            var result = await _orderValidation.InvokeAsync(order, BusinessSetOperation.Update);

            Assert.False(result.ValidationResult.IsValid);
            Assert.Contains("Same product in multi order item", result.ValidationResult.Errors);
            _mockGenericValidationProduct.Verify(m => m.Exists(101, BusinessSetOperation.Get), Times.Once);
            _mockGenericValidationProduct.Verify(m => m.Exists(102, BusinessSetOperation.Get), Times.Once);
            Assert.Equal(2, result.Products.Count);
            Assert.Contains(product101, result.Products);
            Assert.Contains(product102, result.Products);
        }

        [Fact]
        public async Task InvokeAsync_ProductNotFoundInOrderItem_AddsErrorAndContinuesProcessing()
        {
            var order = new Order
            {
                OrderId = 1,
                CustomerId = 10,
                OrderItems = new List<OrderItem>
            {
                new OrderItem { ProductId = 101, Quantity = 1 },
                new OrderItem { ProductId = 102, Quantity = 2 }
            }
            };
            var product101 = new Product { ProductId = 101, Name = "Prod A" };

            _mockGenericValidationProduct.Setup(m => m.Exists(101, BusinessSetOperation.Get))
                .ReturnsAsync(new ExistsResult<Product> { Entity = product101 });
            _mockGenericValidationProduct.Setup(m => m.Exists(102, BusinessSetOperation.Get))
                .ReturnsAsync(new ExistsResult<Product> { Entity = null });

            var result = await _orderValidation.InvokeAsync(order, BusinessSetOperation.Update);

            Assert.False(result.ValidationResult.IsValid);
            Assert.Contains("Product Not Found", result.ValidationResult.Errors);
            _mockGenericValidationProduct.Verify(m => m.Exists(101, BusinessSetOperation.Get), Times.Once);
            _mockGenericValidationProduct.Verify(m => m.Exists(102, BusinessSetOperation.Get), Times.Once);
            Assert.Single(result.Products);
            Assert.Contains(product101, result.Products);
        }

        [Fact]
        public async Task InvokeAsync_AllValidationsPass_ReturnsValidResultWithProducts()
        {
            var order = new Order
            {
                OrderId = 1,
                CustomerId = 10,
                OrderItems = new List<OrderItem>
            {
                new OrderItem { ProductId = 101, Quantity = 1 },
                new OrderItem { ProductId = 102, Quantity = 2 }
            }
            };
            var existingOrder = new Order { OrderId = 1, CustomerId = 10, OrderDate = DateTime.Now };
            var customer = new Customer { CustomerId = 10, FirstName = "Test Customer" };
            var product101 = new Product { ProductId = 101, Name = "Product A", Price = 10.0f };
            var product102 = new Product { ProductId = 102, Name = "Product B", Price = 20.0f };

            _mockGenericValidationOrder.Setup(m => m.Exists(order.OrderId, BusinessSetOperation.Update))
                .ReturnsAsync(new ExistsResult<Order> { Entity = existingOrder });
            _mockGenericValidationCustomer.Setup(m => m.Exists(order.CustomerId, BusinessSetOperation.Get))
                .ReturnsAsync(new ExistsResult<Customer> { Entity = customer });
            _mockGenericValidationProduct.Setup(m => m.Exists(101, BusinessSetOperation.Get))
                .ReturnsAsync(new ExistsResult<Product> { Entity = product101 });
            _mockGenericValidationProduct.Setup(m => m.Exists(102, BusinessSetOperation.Get))
                .ReturnsAsync(new ExistsResult<Product> { Entity = product102 });

            var result = await _orderValidation.InvokeAsync(order, BusinessSetOperation.Update);

            Assert.True(result.ValidationResult.IsValid);
            Assert.Empty(result.ValidationResult.Errors);
            Assert.Same(existingOrder, result.ValidationResult.Entity);
            Assert.Equal(2, result.Products.Count);
            Assert.Contains(product101, result.Products);
            Assert.Contains(product102, result.Products);
        }
    }







}
