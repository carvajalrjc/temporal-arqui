// src/AppHost/Program.cs
using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using Microsoft.EntityFrameworkCore;
using MiniFacturacion.Contracts;       // IAppService
using MiniFacturacion.Infrastructure;  // AppDbContext
using MiniFacturacion.AppHost;         // AppSoapService

var builder = WebApplication.CreateBuilder(args);

// 1) EF Core -> ConnectionStrings:SqlServer (appsettings/env)
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

// 2) Registrar el servicio SOAP en el contenedor (usa constructor con AppDbContext)
builder.Services.AddScoped<AppSoapService>();

// 3) CoreWCF + metadata (WSDL)
builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();
builder.Services.AddSingleton<ServiceMetadataBehavior>(_ => new ServiceMetadataBehavior
{
    HttpGetEnabled = true,
    HttpsGetEnabled = false
});

var app = builder.Build();

// 4) Crear BD/tablas si no existen (para demo). En producción usa Migrate().
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    // db.Database.Migrate(); // si tienes migraciones
}

// 5) Configurar CoreWCF (servicio + endpoint + faults detalladas)
app.UseServiceModel(sm =>
{
    sm.AddService<AppSoapService>();
    sm.AddServiceEndpoint<AppSoapService, IAppService>(
        new BasicHttpBinding(), "/soap/app");

    sm.ConfigureServiceHostBase<AppSoapService>(host =>
    {
        var dbg = host.Description.Behaviors.Find<ServiceDebugBehavior>();
        if (dbg == null)
            host.Description.Behaviors.Add(new ServiceDebugBehavior { IncludeExceptionDetailInFaults = true });
        else
            dbg.IncludeExceptionDetailInFaults = true;
    });
});

app.Run();
