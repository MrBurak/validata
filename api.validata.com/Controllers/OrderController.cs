using AutoMapper;
using business.validata.com.Interfaces;
using model.validata.com.Order;
using data.validata.com.Entities;
using Microsoft.AspNetCore.Mvc;
using model.validata.com;
using model.validata.com.Enumeration;


namespace api.validata.com.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
       

        private readonly ILogger<OrderController> logger;

        private readonly IOrderCommandBusiness commandBusiness;
        private readonly IOrderQueryBusiness queryBusiness;
        private readonly IMapper mapper;
        private readonly MapperConfiguration mapperConfiguration;
        public OrderController(ILogger<OrderController> logger, IOrderQueryBusiness queryBusiness, IOrderCommandBusiness commandBusiness)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(commandBusiness);
            ArgumentNullException.ThrowIfNull(queryBusiness);
            this.logger = logger;
            this.commandBusiness = commandBusiness;
            this.queryBusiness = queryBusiness;

            mapperConfiguration = new MapperConfiguration(mapperConfigurationExpression =>
            {
                mapperConfigurationExpression.CreateMap<OrderInsertModel, OrderUpdateModel>();

            });
            mapper = mapperConfiguration.CreateMapper();
        }

        [HttpGet("{customerid}")]
        public async Task<QueryResult<IEnumerable<OrderViewModel>>> List(int customerid)
        {
            return await queryBusiness.ListAsync(customerid);
        }

        [HttpPost]
        public async Task<CommandResult<OrderDetailViewModel>> Insert(OrderInsertModel request)
        {
            var order = mapper.Map<OrderUpdateModel>(request);

            return await commandBusiness.InvokeAsync(order, BusinessSetOperation.Create);

        }

        [HttpPut]
        public async Task<CommandResult<OrderDetailViewModel>> Update(OrderUpdateModel request)
        {
            return await commandBusiness.InvokeAsync(request, BusinessSetOperation.Update);
        }

        [HttpDelete]
        public async Task<CommandResult<Order>> Delete(int id)
        {
            return await commandBusiness.DeleteAsync(id);
        }

        [HttpGet("{orderId}/{customerId}")]
        public async Task<QueryResult<OrderDetailViewModel?>> Get(int orderId, int customerId)
        {
            return await queryBusiness.GetAsync(orderId, customerId);
        }
    }
}
