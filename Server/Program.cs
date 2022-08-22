using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Polly;
using Polly.Extensions.Http;
using PowerDNS_Auth_CouchDB_Remote_Backend.Brokers;
using PowerDNS_Auth_CouchDB_Remote_Backend.Middleware;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models.Configuration;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models.Services;
using PowerDNS_Auth_CouchDB_Remote_Backend.Services;
using Prometheus;
using Sentry.Extensibility;
using Serilog;
using Serilog.Events;

namespace PowerDNS_Auth_CouchDB_Remote_Backend;

public class Program
{
    private const string outputFormat =
        "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{SourceContext}] {Message:lj} {Exception}{NewLine}";

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        var applicationConfig = builder.Configuration.GetSection(ApplicationConfig.Section).Get<ApplicationConfig>();

        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmm");
        Log.Logger = new LoggerConfiguration().MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information).WriteTo.Async(config =>
            {
                config.File($"Logs/Log{timestamp}.log", outputTemplate: outputFormat,
                    restrictedToMinimumLevel: LogEventLevel.Information);
                config.Console(outputTemplate: outputFormat, restrictedToMinimumLevel: LogEventLevel.Information);
            }).Enrich.FromLogContext().CreateLogger();
        Log.Logger.Information("Loaded SeriLog Logger");


        builder.Host.UseSerilog();
        if (string.IsNullOrWhiteSpace(applicationConfig.SENTRY_DSN) == false)
            builder.WebHost.UseSentry(options =>
            {
                options.Dsn = applicationConfig.SENTRY_DSN;
                options.SendDefaultPii = true;
                options.AttachStacktrace = true;
                options.MaxRequestBodySize = RequestSize.Always;
                options.MinimumBreadcrumbLevel = LogLevel.Debug;
                options.MinimumEventLevel = LogLevel.Warning;
            });


        // Add services to the container.



        builder.Services.Configure<ApplicationConfig>(
            builder.Configuration.GetSection(ApplicationConfig.Section));


        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();


        builder.WebHost.UseKestrel(options =>
        {
            options.AddServerHeader = false;
            if (string.IsNullOrWhiteSpace(applicationConfig.UnixSocketFile) == false)
            {
                if (File.Exists(applicationConfig.UnixSocketFile)) File.Delete(applicationConfig.UnixSocketFile);

                options.ListenUnixSocket(applicationConfig.UnixSocketFile);
            }
        });


        builder.Services.AddScoped<IAPIBroker, APIBroker>();
        builder.Services.AddHttpClient<IAPIBroker, APIBroker>()
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .AddPolicyHandler(GetRetryPolicy());
        builder.Services.AddScoped<IZoneInfoService, ZoneInfoService>();
        builder.Services.AddScoped<IRecordInfoService, RecordInfoService>();
        builder.Services.AddScoped<JSONErrorMiddleware>();
        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = ctx => new ModelStateFilterJSON();
        });


        var app = builder.Build();


        if (applicationConfig.Prometheus_Metrics_Port != default)
        {
            Log.Logger.Information($"Enabling Prometheus Metrics at port {applicationConfig.Prometheus_Metrics_Port}.");
            app.UseMetricServer(applicationConfig.Prometheus_Metrics_Port);
        }


        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            // Log incoming requests with method and headers
            app.Use(async (context, next) =>
            {
                await next.Invoke();
                Log.Information(
                    $"{context.Request.GetDisplayUrl()} - {context.Request.Scheme} - {context.Request.Method} - {string.Join("|", context.Request.Headers.Select(i => $"{i.Key}={i.Value}"))} - Reply: {context.Response.StatusCode}");
            });
        }
        else
        {
            app.UseHttpsRedirection();
        }

        app.UseMiddleware<JSONErrorMiddleware>();

        app.UseAuthorization();

        app.MapControllers();


        if (applicationConfig.Prometheus_Metrics_Port != default) app.UseHttpMetrics();


        // We wouldn't flush any errors above to Log, but Sentry should be fine for that.
        try
        {
            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Web Server died");
            throw;
        }
        finally
        {
            Log.CloseAndFlush();
        }

    }
    static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromMilliseconds(Math.Max(50, retryAttempt * 50)));
    }
}