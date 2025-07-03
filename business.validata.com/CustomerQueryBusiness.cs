using business.validata.com.Interfaces;
using data.validata.com.Interfaces.Repository;
using model.validata.com.Customer;
using Microsoft.Extensions.Logging;
using model.validata.com;
using util.validata.com;
using data.validata.com.Entities;


namespace business.validata.com
{
    public class CustomerQueryBusiness : ICustomerQueryBusiness
    {
        private readonly ICustomerRepository repository;
        public CustomerQueryBusiness(ICustomerRepository repository, ILogger<CustomerQueryBusiness> logger)
        {
            ArgumentNullException.ThrowIfNull(repository);
            this.repository = repository;
        }
        public async Task<QueryResult<IEnumerable<CustomerViewModel>>> ListAsync()
        {
            var queryResult = new QueryResult<IEnumerable<CustomerViewModel>>();
            try
            {
                queryResult.Result= ObjectUtil.ConvertObj<IEnumerable<CustomerViewModel>, IEnumerable<Customer>>(await repository.GetAllAsync());
               
                queryResult.Success = true;
            }
            catch (Exception ex) 
            {
                queryResult.Success = false;
                queryResult.Exception=ex.Message;
            }
           return queryResult;
        }

        public async Task<QueryResult<CustomerViewModel?>> GetAsync(int id)
        {
            var queryResult = new QueryResult<CustomerViewModel?>();
            try 
            {
                var customer = (await repository.GetByIdAsync(id));
                if (customer == null) 
                {
                    queryResult.Exception = "No record found";
                    queryResult.Success = false;
                    return queryResult;
                }
                queryResult.Result = ObjectUtil.ConvertObj<CustomerViewModel, Customer>(customer);
                queryResult.Success = true;
            }
            catch (Exception ex)
            {
                queryResult.Success = false;
                queryResult.Exception = ex.Message;
            }
            return queryResult;

        }
    }
}
