using AutoMapper;
using business.validata.com.Interfaces;
using model.validata.com.Customer;
using data.validata.com.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using model.validata.com;
using model.validata.com.Enumeration;


namespace api.validata.com.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
       

        private readonly ILogger<CustomerController> logger;

        private readonly ICustomerCommandBusiness commandBusiness;
        private readonly ICustomerQueryBusiness queryBusiness;
        private readonly IMapper mapper;
        private readonly MapperConfiguration mapperConfiguration;
        public CustomerController(ILogger<CustomerController> logger, ICustomerCommandBusiness commandBusiness, ICustomerQueryBusiness queryBusiness)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(commandBusiness);
            ArgumentNullException.ThrowIfNull(queryBusiness);
            this.logger = logger;
            this.commandBusiness = commandBusiness;
            this.queryBusiness = queryBusiness;   

            mapperConfiguration= new MapperConfiguration(mapperConfigurationExpression =>
            {
                mapperConfigurationExpression.CreateMap<CustomerInsertModel, Customer>();
                mapperConfigurationExpression.CreateMap<CustomerUpdateModel, Customer>();
            });
            mapper = mapperConfiguration.CreateMapper();
        }

        [HttpGet]
        public async Task<QueryResult<IEnumerable<CustomerViewModel>>> List()
        {
            return await queryBusiness.ListAsync();
        }

        [HttpPost]
        public async Task<CommandResult<CustomerViewModel>> Insert(CustomerInsertModel request)
        {
            var customer = mapper.Map<Customer>(request);

            return await commandBusiness.InvokeAsync(customer, BusinessSetOperation.Create);
        }

        [HttpPut]
        public async Task<CommandResult<CustomerViewModel>> Update(CustomerUpdateModel request)
        {
            var customer = mapper.Map<Customer>(request);
            return await commandBusiness.InvokeAsync(customer, BusinessSetOperation.Update);
        }

        [HttpDelete]
        public async Task<CommandResult<Customer>> Delete(int customerId)
        {
            return await commandBusiness.DeleteAsync(customerId);
        }

        [HttpGet("{customerId}")]
        public async Task<QueryResult<CustomerViewModel?>> Get(int customerId)
        {
            return await queryBusiness.GetAsync(customerId);
        }
    }
}
