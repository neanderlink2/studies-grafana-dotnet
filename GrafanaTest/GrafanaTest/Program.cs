using GrafanaTest.Context;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSqlite<ApplicationContext>("Data Source=app.db");
builder.Services.AddControllers()
    .AddControllersAsServices()
    .AddMvcOptions(options => { })
    .AddNewtonsoftJson();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenTelemetry()
        .WithTracing(builder =>
            builder.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("grafana-test-app"))
                .AddAspNetCoreInstrumentation()
                .AddOtlpExporter(opt =>
                {
                    opt.Endpoint = new Uri("http://grafana-agent:4317");
                })
        )
        .StartWithHost();

var app = builder.Build();

await CheckDbExists(app.Services, app.Logger);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseAuthorization();


app.MapControllers();

app.Run();


static async Task CheckDbExists(IServiceProvider services, ILogger logger)
{
    logger.LogInformation("Garantindo que o banco de dados exista");
    using var db = services.CreateScope().ServiceProvider.GetRequiredService<ApplicationContext>();

    await db.Database.MigrateAsync();
}
