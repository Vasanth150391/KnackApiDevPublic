using Microsoft.EntityFrameworkCore;
using Knack.DBEntities;
using Knack.API.Data;
using Knack.API.Interfaces;
using Knack.API.DataManagers;
using Knack.API.Builders;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Identity.Client;
using Knack.API.Common;
using Knack.API.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("KnackDB");
builder.Services.AddDbContextPool<KnackContext>(option =>
option.UseSqlServer(connectionString)
);

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory()).
    AddJsonFile("appsettings.json",true,true);

// Add services to the container.

builder.Services.AddControllers();
builder.Logging.AddLog4Net();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<Utilities, Utilities>();
builder.Services.AddTransient<IReportManager, ReportManager>();
builder.Services.AddTransient<IReportBuilder, ReportBuilder>();
builder.Services.AddTransient<ITechnologyBuilder, TechnologyBuilder>();
builder.Services.AddTransient<ITechnologyManager, TechnologyManager>();
builder.Services.AddTransient<IUserBuilder, UserBuilder>();
builder.Services.AddTransient<IUserManager, UserManager>();
builder.Services.AddTransient<IPartnerSolutionBuilder, IndustryBuilder>();
builder.Services.AddTransient<IPartnerSolutionManager, PartnerSolutionManager>();
builder.Services.AddTransient<ILeadGeneratorBuilder, LeadGeneratorBuilder>();
builder.Services.AddTransient<ILeadGeneratorManager, LeadGeneratorManager>();
builder.Services.AddTransient<ILookupManager, LookupManager>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddHttpClient();
builder.Services.AddHsts(
    options =>
    {
        options.Preload = true;
        options.IncludeSubDomains = true;
        options.MaxAge = TimeSpan.FromDays(365);
    }
    );

var corsPolicyConfig = builder.Configuration.GetSection("CorsPloicySettings").GetSection("AllowedUrls");

var allowedUrls = corsPolicyConfig?.Get<string[]>();

if (allowedUrls == null || allowedUrls.Length == 0)
    return;
var corsPolicyBuilder = new CorsPolicyBuilder();

foreach (var url in allowedUrls)
{
    corsPolicyBuilder.WithOrigins(url);
}

corsPolicyBuilder.WithMethods();
corsPolicyBuilder.AllowAnyHeader();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(corsPolicyBuilder.Build());

});
builder.Services.AddScoped<Knack.API.Interfaces.ISqlQueryManager, Knack.API.DataManagers.SqlQueryManager>();
builder.Services.AddScoped<Knack.API.Interfaces.ISqlQueryBuilder, Knack.API.Builders.SqlQueryBuilder>();
builder.Services.AddScoped<Knack.API.AzureAI.AzureAIAssistantClient>();
builder.Services.AddScoped<Knack.API.Interfaces.IAIAssistBuilder, Knack.API.Builders.AIAssistBuilder>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHsts();
app.UseStaticFiles();
app.UseRouting();

app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.Run();
