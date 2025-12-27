using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PharmaDiaries.BusinessContract;
using PharmaDiaries.Bussiness;
using PharmaDiaries.DataAccess;
using PharmaDiaries.DataAccessContract;
using PharmaDiaries.DataAccessContract.Repository;
using Microsoft.OpenApi.Models;
using PharmaDiariesAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Swagger/OpenAPI setup
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PharmaDiaries API",
        Version = "v1",
        Description = "API documentation for PharmaDiaries"
    });
});

// Register your dependencies
builder.Services.AddSingleton<IFWEmpDTRepository, FWEmpDTRepository>();
builder.Services.AddSingleton<IFWEmpDTBusiness, FWEmpDTBusiness>();

builder.Services.AddSingleton<IFWHdRepository, FWHdRepository>();
builder.Services.AddSingleton<IFWHDBusiness, FWHDBusiness>();

builder.Services.AddSingleton<ILookupRepository, LookupRepository>();
builder.Services.AddSingleton<ILookupBusiness, LookupBusiness>();

builder.Services.AddSingleton<ILoginRepository, LoginRepository>();
builder.Services.AddSingleton<ILoginBusiness, LoginBusiness>();

builder.Services.AddSingleton<IAreasRepository, AreasRepository>();
builder.Services.AddSingleton<IAreasBusiness, AreasBusiness>();

builder.Services.AddSingleton<ICustomerRepository, CustomerRepository>();
builder.Services.AddSingleton<ICustomerBusiness, CustomerBusiness>();

builder.Services.AddSingleton<IProductRepository, ProductRepository>();
builder.Services.AddSingleton<IProductBusiness, ProductBusiness>();

builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IUserBusiness, UserBusiness>();

builder.Services.AddSingleton<IReportRepository, ReportRepository>();
builder.Services.AddSingleton<IReportBusiness, ReportBusiness>();

builder.Services.AddSingleton<ICompanyRepository, CompanyRepository>();
builder.Services.AddSingleton<ICompanyBusiness, CompanyBusiness>();

builder.Services.AddSingleton<IScreenRepository, ScreenRepository>();
builder.Services.AddSingleton<IScreenBusiness, ScreenBusiness>();

// R2 Storage Service for image uploads
builder.Services.AddSingleton<IR2StorageService, R2StorageService>();

// Orders Repository (POB - Personal Order Booking)
builder.Services.AddSingleton<IOrdersRepository, OrdersRepository>();

// DCR Date Request Repository (Past date DCR approval)
builder.Services.AddSingleton<IDCRDateRequestRepository, DCRDateRequestRepository>();

// Enable CORS
builder.Services.AddCors(c =>
{
    c.AddPolicy("AllowOrigin", options =>
    {
        options.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PharmaDiaries API v1");
        //c.RoutePrefix = string.Empty; // Swagger at root (optional)
    });
}

// app.UseHttpsRedirection(); // enable later if needed

app.UseRouting();

app.UseCors("AllowOrigin");

app.UseAuthorization();

app.MapControllers();

app.Run();
