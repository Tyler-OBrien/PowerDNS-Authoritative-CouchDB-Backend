using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using PowerDNS_Auth_CouchDB_Remote_Backend;
using PowerDNS_Auth_CouchDB_Remote_Backend.Brokers;
using PowerDNS_Auth_CouchDB_Remote_Backend.Middleware;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models.Services;
using PowerDNS_Auth_CouchDB_Remote_Backend.Services;

namespace IntegrationTests.Fixtures;

public class TestingWebAppFactory<TEntryPoint> : WebApplicationFactory<Program> where TEntryPoint : Program
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton<IAPIBroker, APIBrokerInMemory>();
            services.AddScoped<IZoneInfoService, ZoneInfoService>();
            services.AddScoped<IRecordInfoService, RecordInfoService>();
            services.AddScoped<JSONErrorMiddleware>();
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = ctx => new ModelStateFilterJSON();
            });
        });
    }
}