using Smartwyre.DeveloperTest.Data.Interfaces;
using Smartwyre.DeveloperTest.Domain.Types;
using System;
using System.Threading.Tasks;

namespace Smartwyre.DeveloperTest.Runner;

// Runner-only implementations used for manual testing/demo
public sealed class InMemoryRebateRepository : IRebateRepository
{
    public Task<Rebate> GetRebate(string rebateIdentifier)
    {
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

public sealed class InMemoryProductRepository : IProductRepository
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