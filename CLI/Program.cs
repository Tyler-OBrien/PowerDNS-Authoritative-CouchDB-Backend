using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using CLI.Broker;
using CLI.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace CLI;

public static class Program
{
    /// <summary>
    ///     The entry point for the program.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <returns>When complete, an integer representing success (0) or failure (non-0).</returns>
    public static async Task<int> Main(string[] args)
    {
        var serviceProvider = BuildServiceProvider();
        var parser = BuildParser(serviceProvider);

        return await parser.InvokeAsync(args).ConfigureAwait(false);
    }

    private static Parser BuildParser(ServiceProvider serviceProvider)
    {
        var commandLineBuilder = new CommandLineBuilder();

        foreach (var command in serviceProvider.GetServices<Command>()) commandLineBuilder.Command.AddCommand(command);

        return commandLineBuilder.UseDefaults().Build();
    }

    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();

        services.AddHttpClient();

        services.AddScoped<IAPIBroker, APIBroker>();

        services.AddCliCommands();

        return services.BuildServiceProvider();
    }
}