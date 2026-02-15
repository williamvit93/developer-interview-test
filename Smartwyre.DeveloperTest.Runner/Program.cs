using Smartwyre.DeveloperTest.Data.Interfaces;
using Smartwyre.DeveloperTest.Domain.Incentives;
using Smartwyre.DeveloperTest.Domain.Incentives.Interfaces;
using Smartwyre.DeveloperTest.Domain.Types;
using Smartwyre.DeveloperTest.Services;
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
        // Build simple in-memory repositories and calculators for runner execution.
        IRebateRepository rebateRepository = new InMemoryRebateRepository();
        IProductRepository productRepository = new InMemoryProductRepository();

        var calculators = new IIncentiveCalculator[]
        {
            new FixedCashAmountCalculator(),
            new FixedRateRebateCalculator(),
            new AmountPerUomCalculator()
        };

        var service = new RebateService(rebateRepository, productRepository, calculators);

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
        var result = await service.Calculate(request);

        Console.WriteLine();
        Console.WriteLine($"Success: {result.Success}");
        Console.WriteLine($"Amount: {result.Amount:C}");
    }

    private static decimal ParseDecimalOrDefault(string s) =>
        decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var v) ? v : 0m;

    // --- Simple in-memory implementations used only by the Runner ---
    private sealed class InMemoryRebateRepository : IRebateRepository
    {
        public Task<Rebate> GetRebate(string rebateIdentifier)
        {
            // Return some example rebates
            return rebateIdentifier switch
            {
                "R_FIXED" => Task.FromResult(new Rebate
                {
                    Identifier = "R_FIXED",
                    Incentive = IncentiveType.FixedCashAmount,
                    Amount = 10m
                }),
                "R_RATE" => Task.FromResult(new Rebate
                {
                    Identifier = "R_RATE",
                    Incentive = IncentiveType.FixedRateRebate,
                    Percentage = 0.10m
                }),
                "R_PERUOM" => Task.FromResult(new Rebate
                {
                    Identifier = "R_PERUOM",
                    Incentive = IncentiveType.AmountPerUom,
                    Amount = 2m
                }),
                _ => Task.FromResult<Rebate>(null!)
            };
        }

        public Task StoreCalculationResult(Rebate rebate, decimal amount)
        {
            // For the runner we simply write to console; a real implementation would persist.
            Console.WriteLine();
            Console.WriteLine("Storing calculation result (in-memory):");
            Console.WriteLine($"Rebate: {rebate?.Identifier ?? "null"}, Incentive: {rebate?.Incentive}, Amount: {amount:C}");
            return Task.CompletedTask;
        }
    }

    private sealed class InMemoryProductRepository : IProductRepository
    {
        public Task<Product> GetProduct(string productIdentifier)
        {
            return productIdentifier switch
            {
                "P_FIXED" => Task.FromResult(new Product
                {
                    Identifier = "P_FIXED",
                    Price = 0m,
                    Uom = "EA",
                    SupportedIncentives = SupportedIncentiveType.FixedCashAmount
                }),
                "P_RATE" => Task.FromResult(new Product
                {
                    Identifier = "P_RATE",
                    Price = 100m,
                    Uom = "EA",
                    SupportedIncentives = SupportedIncentiveType.FixedRateRebate
                }),
                "P_UOM" => Task.FromResult(new Product
                {
                    Identifier = "P_UOM",
                    Price = 0m,
                    Uom = "KG",
                    SupportedIncentives = SupportedIncentiveType.AmountPerUom
                }),
                "P_ALL" => Task.FromResult(new Product
                {
                    Identifier = "P_ALL",
                    Price = 50m,
                    Uom = "EA",
                    SupportedIncentives = SupportedIncentiveType.FixedCashAmount | SupportedIncentiveType.FixedRateRebate | SupportedIncentiveType.AmountPerUom
                }),
                _ => Task.FromResult<Product>(null!)
            };
        }
    }
}