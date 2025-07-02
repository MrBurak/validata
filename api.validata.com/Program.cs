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


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


#region Database DI
builder.Services.AddDbContext<ValidataDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("validataconnectionstring")));
builder.Services.AddTransient(typeof(IDataRepository<>), typeof(SqlRepository<>));
builder.Services.AddSingleton<IMetadata, Metadata>(serviceProvider => MetadataFactory.Create());
#endregion

#region Business DI
builder.Services.AddTransient<IGenericLambdaExpressions, GenericLambdaExpressions>();
builder.Services.AddTransient(typeof(IGenericValidation<>), typeof(GenericValidation<>));
builder.Services.AddTransient(typeof(IStringFieldValidation<>), typeof(StringFieldValidation<>));

builder.Services.AddTransient<ICustomerValidation, CustomerValidation>();
builder.Services.AddTransient<ICustomerBusiness, CustomerBusiness>();
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
