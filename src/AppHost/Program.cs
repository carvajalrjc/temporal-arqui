using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using Microsoft.EntityFrameworkCore;
using MiniFacturacion.Infrastructure;
using MiniFacturacion.AppHost;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();
builder.Services.AddScoped<AppSoapService>();
builder.Services.AddSingleton<ServiceMetadataBehavior>(_ => new ServiceMetadataBehavior { HttpGetEnabled = true });

var app = builder.Build();

app.UseServiceModel(sm =>
{
    sm.AddService<AppSoapService>();
    sm.AddServiceEndpoint<AppSoapService, IAppService>(new BasicHttpBinding(), "/soap/app");
});

app.Run();
