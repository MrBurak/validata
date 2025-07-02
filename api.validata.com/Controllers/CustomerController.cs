using AutoMapper;
using Azure.Core;
using business.validata.com.Interfaces;
using customer.validata.com.Models;
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

        private readonly ICustomerBusiness business;
        private readonly IMapper mapper;
        private readonly MapperConfiguration mapperConfiguration;
        public CustomerController(ILogger<CustomerController> logger, ICustomerBusiness business)
        {
            this.logger = logger;
            this.business = business;
            mapperConfiguration= new MapperConfiguration(mapperConfigurationExpression =>
            {
                mapperConfigurationExpression.CreateMap<CustomerInsertModel, Customer>();
                mapperConfigurationExpression.CreateMap<CustomerUpdateModel, Customer>();
                mapperConfigurationExpression.CreateMap<Customer, CustomerViewModel>();
            });
            mapper = mapperConfiguration.CreateMapper();
        }

        [HttpGet]
        public async Task<IEnumerable<CustomerViewModel>> List()
        {
            var customers = await business.GetListAsync();
            return customers.Select(x => new CustomerViewModel 
            { 
                CustomerId = x.CustomerId, 
                Email = x.Email,
                Address=x.Address,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Pobox = x.Pobox
            });
        }

        [HttpPost]
        public async Task<ApiResult<Customer>> Insert(CustomerInsertModel request)
        {
            var customer = mapper.Map<Customer>(request);

            return await business.InvokeAsync(customer, BusinessSetOperation.Create);
        }

        [HttpPut]
        public async Task<ApiResult<Customer>> Update(CustomerUpdateModel request)
        {
            var customer = mapper.Map<Customer>(request);
            return await business.InvokeAsync(customer, BusinessSetOperation.Update);
        }

        [HttpDelete]
        public async Task<ApiResult<Customer>> Delete(int id)
        {
            return await business.DeleteAsync(id);
        }

        [HttpGet("{id}")]
        public async Task<CustomerViewModel?> Get(int id)
        {
            var customer= await business.GetEntityAsync(id);
            return mapper.Map<CustomerViewModel>(customer);
        }
    }
}
