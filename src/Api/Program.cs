using CoreWCF;
using CoreWCF.Configuration;   // paquete CoreWCF.ConfigurationManager
using CoreWCF.Description;     // ServiceMetadataBehavior
using Microsoft.EntityFrameworkCore;
using MiniFacturacion.Api;
using MiniFacturacion.Application;
using MiniFacturacion.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// EF Core
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

// DI
builder.Services.AddScoped<IClientesRepository, ClientesRepository>();
builder.Services.AddScoped<IFacturasRepository, FacturasRepository>();
builder.Services.AddScoped<IClientesService, ClientesService>();
builder.Services.AddScoped<IFacturasService, FacturasService>();
builder.Services.AddSingleton<IMontosService, MontosService>();

// 👉 REGISTRA EL SERVICIO SOAP EN DI
// Usa Scoped (o Transient). No uses Singleton porque inyectas servicios Scoped.
builder.Services.AddScoped<MiniFacturacionService>();

// CoreWCF + WSDL (sin MEX)
builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();
builder.Services.AddSingleton<ServiceMetadataBehavior>(_ => new ServiceMetadataBehavior
{
    HttpGetEnabled = true,   // habilita ?wsdl
    HttpsGetEnabled = false
});

builder.Services.AddCors(o => o.AddPolicy("astro", p => p
    .WithOrigins("http://localhost:4321") // dev Astro
    .AllowAnyHeader()
    .AllowAnyMethod()
));

var app = builder.Build();

app.UseCors("astro"); // solo permite que Astro haga POST SOAP

app.UseServiceModel(sm =>
{
    sm.AddService<MiniFacturacionService>();
    sm.AddServiceEndpoint<MiniFacturacionService, IMiniFacturacionService>(
        new BasicHttpBinding(), "/soap/mini");
});

app.Run();
