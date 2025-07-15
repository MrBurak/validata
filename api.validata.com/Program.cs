using data.validata.com.Metadata;
using business.validata.com.Interfaces.Validators;
using business.validata.com.Validators;
using data.validata.com.Context;
using data.validata.com.Interfaces.Metadata;
using data.validata.com.Interfaces.Repository;
using data.validata.com.Repositories;
using Microsoft.EntityFrameworkCore;
using business.validata.com.Utils;
using business.validata.com.Interfaces.Utils;
using business.validata.com.Interfaces;
using business.validata.com;
using business.validata.com.Adaptors;
using business.validata.com.Interfaces.Adaptors;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


#region Database DI
builder.Services.AddDbContext<CommandContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("validataconnectionstring")));
builder.Services.AddSingleton<QueryContext>();
builder.Services.AddTransient(typeof(ICommandRepository<>), typeof(CommandRepository<>));

builder.Services.AddTransient(typeof(ICustomerRepository), typeof(CustomerRepository));
builder.Services.AddTransient(typeof(IProductRepository), typeof(ProductRepository));
builder.Services.AddTransient(typeof(IOrderRepository), typeof(OrderRepository));
builder.Services.AddTransient(typeof(IOrderItemRepository), typeof(OrderItemRepository));
builder.Services.AddSingleton<IMetadata, Metadata>(serviceProvider => MetadataFactory.Create());
#endregion

#region Business DI
builder.Services.AddTransient<IGenericLambdaExpressions, GenericLambdaExpressions>();
builder.Services.AddTransient(typeof(IGenericValidation<>), typeof(GenericValidation<>));
builder.Services.AddTransient(typeof(IStringFieldValidation<>), typeof(StringFieldValidation<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<ICustomerValidation, CustomerValidation>();
builder.Services.AddTransient<ICustomerCommandBusiness, CustomerCommandBusiness>();
builder.Services.AddTransient<ICustomerQueryBusiness, CustomerQueryBusiness>();
builder.Services.AddTransient<ICustomerAdaptor, CustomerAdaptor>();
builder.Services.AddTransient<IProductValidation, ProductValidation>();
builder.Services.AddTransient<IProductCommandBusiness, ProductCommandBusiness>();
builder.Services.AddTransient<IProductQueryBusiness, ProductQueryBusiness>();
builder.Services.AddTransient<IProductAdaptor, ProductAdaptor>();
builder.Services.AddTransient<IOrderValidation, OrderValidation>();
builder.Services.AddTransient<IOrderQueryBusiness, OrderQueryBusiness>();
builder.Services.AddTransient<IOrderCommandBusiness, OrderCommandBusiness>();
builder.Services.AddTransient<IOrderAdaptor, OrderAdaptor>();

#endregion



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
