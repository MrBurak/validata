using business.validata.com;
using business.validata.com.Interfaces;
using business.validata.com.Interfaces.Utils;
using business.validata.com.Interfaces.Validators;
using data.validata.com.Entities;
using data.validata.com.Interfaces.Repository;
using model.validata.com;
using model.validata.com.Enumeration;
using model.validata.com.Validators;
using Moq;
using System.Linq.Expressions;

namespace business.validata.test
{
    public class CustomerCommandBusinessTests
    {
        private readonly Mock<ICustomerValidation> _mockCustomerValidation;
        private readonly Mock<ICommandRepository<Customer>> _mockRepository;
        private readonly Mock<IGenericValidation<Customer>> _mockGenericValidation;
        private readonly Mock<IGenericLambdaExpressions> _mockGenericLambdaExpressions;
        private readonly Mock<IOrderCommandBusiness> _mockOrderCommandBusiness;
        private readonly CustomerCommandBusiness _customerCommandBusiness;

        public CustomerCommandBusinessTests()
        {
            _mockCustomerValidation = new Mock<ICustomerValidation>();
            _mockRepository = new Mock<ICommandRepository<Customer>>();
            _mockGenericValidation = new Mock<IGenericValidation<Customer>>();
            _mockGenericLambdaExpressions = new Mock<IGenericLambdaExpressions>();
            _mockOrderCommandBusiness = new Mock<IOrderCommandBusiness>();

            _customerCommandBusiness = new CustomerCommandBusiness(
                _mockCustomerValidation.Object,
                _mockRepository.Object,
                _mockGenericValidation.Object,
                _mockGenericLambdaExpressions.Object,
                _mockOrderCommandBusiness.Object
            );

            _mockCustomerValidation.Setup(v => v.InvokeAsync(It.IsAny<Customer>(), It.IsAny<BusinessSetOperation>()))
                .ReturnsAsync(new ValidationResult<Customer> { Entity = new Customer { CustomerId = 1, FirstName = "John", LastName = "Doe" } });

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Customer>()))
                .ReturnsAsync(new Customer { CustomerId = 1, FirstName = "John", LastName = "Doe", Address = "123 Main St", Pobox = "PO Box 1" });
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<List<Action<Customer>>>()))
                .Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.GetEntityAsync(It.IsAny<Expression<Func<Customer, bool>>>()))
                .ReturnsAsync(new Customer { CustomerId = 1, FirstName = "John", LastName = "Doe", Address = "123 Main St", Pobox = "PO Box 1" });

            _mockGenericValidation.Setup(gv => gv.Exists(It.IsAny<int>(), It.IsAny<BusinessSetOperation>()))
                .ReturnsAsync(new ExistsResult<Customer> { Entity = new Customer { CustomerId = 1 } });

            _mockGenericLambdaExpressions.Setup(gle => gle.GetEntityById<Customer>(It.IsAny<int>()))
                .Returns<int>(id => c => c.CustomerId == id);
            _mockGenericLambdaExpressions.Setup(gle => gle.GetEntityByPrimaryKey(It.IsAny<Customer>()))
                .Returns<Customer>(c => cust => cust.CustomerId == c.CustomerId);

            _mockOrderCommandBusiness.Setup(ocb => ocb.DeleteAllAsync(It.IsAny<int>())).Returns(Task.FromResult(new CommandResult<List<Order>> { Success = true }));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfValidationIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new CustomerCommandBusiness(
                null!, _mockRepository.Object, _mockGenericValidation.Object, _mockGenericLambdaExpressions.Object, _mockOrderCommandBusiness.Object));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfOrderCommandBusinessIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new CustomerCommandBusiness(
                _mockCustomerValidation.Object, _mockRepository.Object, _mockGenericValidation.Object, _mockGenericLambdaExpressions.Object, null!));
        }

        [Fact]
        public async Task InvokeAsync_ValidationFails_ReturnsErrors()
        {
            var err = new ValidationResult<Customer>();
            err.AddError("Name is required");
            var customer = new Customer();
            _mockCustomerValidation.Setup(v => v.InvokeAsync(customer, BusinessSetOperation.Create))
                .ReturnsAsync(err);

            var result = await _customerCommandBusiness.InvokeAsync(customer, BusinessSetOperation.Create);

            Assert.False(result.Success);
            Assert.Contains("Name is required", result.Validations);
            Assert.Null(result.Result);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Customer>()), Times.Never);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<List<Action<Customer>>>()), Times.Never);
        }

        

        [Fact]
        public async Task InvokeAsync_UpdateOperation_CallsUpdateAsyncAndReturnsSuccess()
        {
            var customer = new Customer { CustomerId = 1, FirstName = "Updated", LastName = "Customer" };
            var validatedCustomer = new Customer { CustomerId = 1, FirstName = "Original", LastName = "Name" };
            var updatedCustomerInDb = new Customer { CustomerId = 1, FirstName = "Updated", LastName = "Customer", Address = "456 Pine", Pobox = "PO2" };

            _mockCustomerValidation.Setup(v => v.InvokeAsync(customer, BusinessSetOperation.Update))
                .ReturnsAsync(new ValidationResult<Customer> { Entity = validatedCustomer });
            _mockRepository.Setup(r => r.GetEntityAsync(It.IsAny<Expression<Func<Customer, bool>>>()))
                .ReturnsAsync(updatedCustomerInDb);

            var result = await _customerCommandBusiness.InvokeAsync(customer, BusinessSetOperation.Update);

            Assert.True(result.Success);
            Assert.Empty(result.Validations);
            Assert.NotNull(result.Result);
            Assert.Equal(1, result.Result!.CustomerId);

            _mockRepository.Verify(r => r.UpdateAsync(
                It.Is<Expression<Func<Customer, bool>>>(exp => exp.Compile().Invoke(new Customer { CustomerId = 1 })),
                It.Is<List<Action<Customer>>>(list => list.Count == 1)), Times.Once);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Customer>()), Times.Never);
        }


        [Fact]
        public async Task InvokeAsync_HandlesException_ReturnsFailure()
        {
            var customer = new Customer { CustomerId = 1, FirstName = "Error", LastName = "Customer" };
            var expectedExceptionMessage = "Database connection lost";

            _mockCustomerValidation.Setup(v => v.InvokeAsync(customer, BusinessSetOperation.Update))
                .ReturnsAsync(new ValidationResult<Customer> { Entity = new Customer { CustomerId = 1 } });
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<List<Action<Customer>>>()))
                .ThrowsAsync(new Exception(expectedExceptionMessage));

            var result = await _customerCommandBusiness.InvokeAsync(customer, BusinessSetOperation.Update);

            Assert.False(result.Success);
            Assert.Equal(expectedExceptionMessage, result.Exception);
            Assert.Null(result.Result);
        }

        [Fact]
        public async Task DeleteAsync_CallsBaseDeleteAndOrderCommandBusinessDeleteAll()
        {
            int customerIdToDelete = 1;
            var baseDeleteResult = new CommandResult<Customer> { Success = true };

            _mockGenericValidation.Setup(gv => gv.Exists(customerIdToDelete, BusinessSetOperation.Delete))
                .ReturnsAsync(new ExistsResult<Customer> { Entity = new Customer { CustomerId = customerIdToDelete } });
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<List<Action<Customer>>>()))
                .Returns(Task.CompletedTask);

            var result = await _customerCommandBusiness.DeleteAsync(customerIdToDelete);

            _mockRepository.Verify(r => r.UpdateAsync(
                It.Is<Expression<Func<Customer, bool>>>(exp => exp.Compile().Invoke(new Customer { CustomerId = customerIdToDelete })),
                It.IsAny<List<Action<Customer>>>()), Times.Once);
            _mockOrderCommandBusiness.Verify(ocb => ocb.DeleteAllAsync(customerIdToDelete), Times.Once);
            Assert.True(result.Success);
        }

        [Fact]
        public async Task DeleteAsync_BaseDeleteFails_DoesNotCallOrderCommandBusinessDeleteAll()
        {
            int customerIdToDelete = 1;
            var baseDeleteResult = new CommandResult<Customer> { Success = false, Validations = { "Customer not found" } };

            _mockGenericValidation.Setup(gv => gv.Exists(customerIdToDelete, BusinessSetOperation.Delete))
                .ReturnsAsync(new ExistsResult<Customer> { Entity = null });

            var result = await _customerCommandBusiness.DeleteAsync(customerIdToDelete);

            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<List<Action<Customer>>>()), Times.Never);
            _mockOrderCommandBusiness.Verify(ocb => ocb.DeleteAllAsync(It.IsAny<int>()), Times.Once);
            Assert.False(result.Success);
        }
    }
}

