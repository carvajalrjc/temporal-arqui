using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using MiniFacturacion.Contracts;

var builder = WebApplication.CreateBuilder(args);

// CORS sólo para el front
builder.Services.AddCors(o => o.AddPolicy("astro", p => p
    .WithOrigins("http://localhost:4321")
    .AllowAnyHeader()
    .AllowAnyMethod()
));

// CoreWCF (servidor)
builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();
builder.Services.AddSingleton<ServiceMetadataBehavior>(_ => new ServiceMetadataBehavior
{
    HttpGetEnabled = true
});

// Cliente SOAP (WCF clásico) hacia el SAD
builder.Services.AddScoped<IAppService>(_ =>
{
    var binding = new System.ServiceModel.BasicHttpBinding();
    var address = new System.ServiceModel.EndpointAddress(
        Environment.GetEnvironmentVariable("SAD_URL") ?? "http://app:8081/soap/app"
    );
    var factory = new System.ServiceModel.ChannelFactory<IAppService>(binding, address);
    return factory.CreateChannel();
});

// Servicio gateway que implementa el contrato y reenvía al SAD
builder.Services.AddScoped<MiniFacturacion.Api.GatewaySoapService>();

var app = builder.Build();

app.UseCors("astro");

app.UseServiceModel(sm =>
{
    sm.AddService<MiniFacturacion.Api.GatewaySoapService>();

    sm.AddServiceEndpoint<MiniFacturacion.Api.GatewaySoapService, IAppService>(
        new CoreWCF.BasicHttpBinding(), "/soap/mini"
    );

    // 🔎 Enviar detalles de excepción en las Faults
    sm.ConfigureServiceHostBase<MiniFacturacion.Api.GatewaySoapService>(host =>
    {
        var debug = host.Description.Behaviors.Find<ServiceDebugBehavior>();
        if (debug == null)
        {
            host.Description.Behaviors.Add(new ServiceDebugBehavior
            {
                IncludeExceptionDetailInFaults = true
            });
        }
        else
        {
            debug.IncludeExceptionDetailInFaults = true;
        }
    });
});



app.Run();
