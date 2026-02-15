using Microsoft.Extensions.DependencyInjection;
using Smartwyre.DeveloperTest.Domain.Types;
using Smartwyre.DeveloperTest.Services.Interfaces;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Smartwyre.DeveloperTest.Runner;

class Program
{
    static void Main(string[] args)
    {
        RunAsync(args).GetAwaiter().GetResult();
    }

    private static async Task RunAsync(string[] args)
    {
        var services = new ServiceCollection();
        services.AddRunnerServices();
        using var provider = services.BuildServiceProvider();

        var service = provider.GetRequiredService<IRebateService>();

        Console.WriteLine("Smartwyre Rebate Runner");
        Console.WriteLine("Sample rebate identifiers: R_FIXED, R_RATE, R_PERUOM");
        Console.WriteLine("Sample product identifiers: P_FIXED, P_RATE, P_UOM, P_ALL");
        Console.WriteLine();

        string rebateIdentifier;
        string productIdentifier;
        decimal volume;

        if (args?.Length >= 3)
        {
            rebateIdentifier = args[0];
            productIdentifier = args[1];
            volume = ParseDecimalOrDefault(args[2]);
        }
        else
        {
            Console.Write("Enter Rebate Identifier: ");
            rebateIdentifier = Console.ReadLine() ?? string.Empty;

            Console.Write("Enter Product Identifier: ");
            productIdentifier = Console.ReadLine() ?? string.Empty;

            Console.Write("Enter Volume (decimal): ");
            var volInput = Console.ReadLine() ?? "0";
            volume = ParseDecimalOrDefault(volInput);
        }

        var request = new CalculateRebateRequest
        {
            RebateIdentifier = rebateIdentifier,
            ProductIdentifier = productIdentifier,
            Volume = volume
        };

        Console.WriteLine();
        Console.WriteLine("Calculating rebate...");

        try
        {
            var result = await service.Calculate(request);

            Console.WriteLine();
            Console.WriteLine($"Success: {result.Success}");
            Console.WriteLine($"Amount: {result.Amount:C}");
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine($"An error occurred while calculating rebate: {ex.Message}");
        }
    }

    private static decimal ParseDecimalOrDefault(string s) =>
        decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var v) ? v : 0m;
}