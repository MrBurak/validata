using AutoMapper;
using business.validata.com.Interfaces;
using model.validata.com.Product;
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
    public class ProductController : ControllerBase
    {
       

        private readonly ILogger<ProductController> logger;

        private readonly IProductCommandBusiness commandBusiness;
        private readonly IProductQueryBusiness queryBusiness;
        private readonly IMapper mapper;
        private readonly MapperConfiguration mapperConfiguration;
        public ProductController(ILogger<ProductController> logger, IProductCommandBusiness commandBusiness, IProductQueryBusiness queryBusiness)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(commandBusiness);
            ArgumentNullException.ThrowIfNull(queryBusiness);
            this.logger = logger;
            this.commandBusiness = commandBusiness;
            this.queryBusiness = queryBusiness;   

            mapperConfiguration= new MapperConfiguration(mapperConfigurationExpression =>
            {
                mapperConfigurationExpression.CreateMap<ProductBaseModel, Product>();
                mapperConfigurationExpression.CreateMap<ProductModel, Product>();
                mapperConfigurationExpression.CreateMap<Product, ProductModel>();

            });
            mapper = mapperConfiguration.CreateMapper();
        }

        [HttpGet]
        public async Task<QueryResult<IEnumerable<ProductModel>>> List()
        {
            return await queryBusiness.ListAsync();
        }

        [HttpPost]
        public async Task<CommandResult<ProductModel>> Insert(ProductBaseModel request)
        {
            var product = mapper.Map<Product>(request);

            return await commandBusiness.InvokeAsync(product, BusinessSetOperation.Create);

        }

        [HttpPut]
        public async Task<CommandResult<ProductModel>> Update(ProductModel request)
        {
            var Product = mapper.Map<Product>(request);
            return await commandBusiness.InvokeAsync(Product, BusinessSetOperation.Update);
        }

        [HttpDelete]
        public async Task<CommandResult<Product>> Delete(int id)
        {
            return await commandBusiness.DeleteAsync(id);
        }

        [HttpGet("{id}")]
        public async Task<QueryResult<ProductModel?>> Get(int id)
        {
            return await queryBusiness.GetAsync(id);
        }
    }
}
