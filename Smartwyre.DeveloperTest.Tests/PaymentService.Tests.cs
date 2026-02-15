using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Smartwyre.DeveloperTest.Data.Interfaces;
using Smartwyre.DeveloperTest.Domain.Incentives;
using Smartwyre.DeveloperTest.Domain.Incentives.Interfaces;
using Smartwyre.DeveloperTest.Domain.Types;
using Smartwyre.DeveloperTest.Services;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests;

public class PaymentServiceTests
{
    [Fact]
    public async Task FixedCashAmount_Succeeds_StoresResult()
    {
        var rebate = new Rebate { Identifier = "R_FIXED", Incentive = IncentiveType.FixedCashAmount, Amount = 15m };
        var product = new Product { Identifier = "P_FIXED", SupportedIncentives = SupportedIncentiveType.FixedCashAmount };

        var rebateRepo = new FakeRebateRepository(new Dictionary<string, Rebate> { ["R_FIXED"] = rebate });
        var productRepo = new FakeProductRepository(new Dictionary<string, Product> { ["P_FIXED"] = product });

        var calculators = new IIncentiveCalculator[] { new FixedCashAmountCalculator(), new FixedRateRebateCalculator(), new AmountPerUomCalculator() };
        var svc = new RebateService(rebateRepo, productRepo, calculators);

        var request = new CalculateRebateRequest { RebateIdentifier = "R_FIXED", ProductIdentifier = "P_FIXED", Volume = 1m };

        var result = await svc.Calculate(request);

        Assert.True(result.Success);
        Assert.Equal(15m, result.Amount);
        Assert.True(rebateRepo.WasStoreCalled);
        Assert.Equal(15m, rebateRepo.StoredAmount);
    }

    [Fact]
    public async Task FixedRateRebate_Succeeds_StoresResult()
    {
        var rebate = new Rebate { Identifier = "R_RATE", Incentive = IncentiveType.FixedRateRebate, Percentage = 0.10m };
        var product = new Product { Identifier = "P_RATE", Price = 100m, SupportedIncentives = SupportedIncentiveType.FixedRateRebate };

        var rebateRepo = new FakeRebateRepository(new Dictionary<string, Rebate> { ["R_RATE"] = rebate });
        var productRepo = new FakeProductRepository(new Dictionary<string, Product> { ["P_RATE"] = product });

        var calculators = new IIncentiveCalculator[] { new FixedCashAmountCalculator(), new FixedRateRebateCalculator(), new AmountPerUomCalculator() };
        var svc = new RebateService(rebateRepo, productRepo, calculators);

        var request = new CalculateRebateRequest { RebateIdentifier = "R_RATE", ProductIdentifier = "P_RATE", Volume = 2m };

        var result = await svc.Calculate(request);

        Assert.True(result.Success);
        Assert.Equal(100m * 0.10m * 2m, result.Amount);
        Assert.True(rebateRepo.WasStoreCalled);
        Assert.Equal(result.Amount, rebateRepo.StoredAmount);
    }

    [Fact]
    public async Task AmountPerUom_Succeeds_StoresResult()
    {
        var rebate = new Rebate { Identifier = "R_PERUOM", Incentive = IncentiveType.AmountPerUom, Amount = 2m };
        var product = new Product { Identifier = "P_UOM", SupportedIncentives = SupportedIncentiveType.AmountPerUom };

        var rebateRepo = new FakeRebateRepository(new Dictionary<string, Rebate> { ["R_PERUOM"] = rebate });
        var productRepo = new FakeProductRepository(new Dictionary<string, Product> { ["P_UOM"] = product });

        var calculators = new IIncentiveCalculator[] { new FixedCashAmountCalculator(), new FixedRateRebateCalculator(), new AmountPerUomCalculator() };
        var svc = new RebateService(rebateRepo, productRepo, calculators);

        var request = new CalculateRebateRequest { RebateIdentifier = "R_PERUOM", ProductIdentifier = "P_UOM", Volume = 3m };

        var result = await svc.Calculate(request);

        Assert.True(result.Success);
        Assert.Equal(2m * 3m, result.Amount);
        Assert.True(rebateRepo.WasStoreCalled);
        Assert.Equal(result.Amount, rebateRepo.StoredAmount);
    }

    [Fact]
    public async Task RebateNotFound_ReturnsFalse()
    {
        var product = new Product { Identifier = "P_ANY", SupportedIncentives = SupportedIncentiveType.FixedCashAmount | SupportedIncentiveType.FixedRateRebate | SupportedIncentiveType.AmountPerUom };

        var rebateRepo = new FakeRebateRepository(new Dictionary<string, Rebate>()); // empty -> rebate not found
        var productRepo = new FakeProductRepository(new Dictionary<string, Product> { ["P_ANY"] = product });

        var calculators = new IIncentiveCalculator[] { new FixedCashAmountCalculator(), new FixedRateRebateCalculator(), new AmountPerUomCalculator() };
        var svc = new RebateService(rebateRepo, productRepo, calculators);

        var request = new CalculateRebateRequest { RebateIdentifier = "UNKNOWN", ProductIdentifier = "P_ANY", Volume = 1m };

        var result = await svc.Calculate(request);

        Assert.False(result.Success);
        Assert.False(rebateRepo.WasStoreCalled);
    }

    [Fact]
    public async Task UnsupportedIncentive_ReturnsFalse()
    {
        var rebate = new Rebate { Identifier = "R_FIXED", Incentive = IncentiveType.FixedCashAmount, Amount = 10m };
        var product = new Product { Identifier = "P_OTHER", SupportedIncentives = SupportedIncentiveType.FixedRateRebate }; // does not support fixed cash

        var rebateRepo = new FakeRebateRepository(new Dictionary<string, Rebate> { ["R_FIXED"] = rebate });
        var productRepo = new FakeProductRepository(new Dictionary<string, Product> { ["P_OTHER"] = product });

        var calculators = new IIncentiveCalculator[] { new FixedCashAmountCalculator(), new FixedRateRebateCalculator(), new AmountPerUomCalculator() };
        var svc = new RebateService(rebateRepo, productRepo, calculators);

        var request = new CalculateRebateRequest { RebateIdentifier = "R_FIXED", ProductIdentifier = "P_OTHER", Volume = 1m };

        var result = await svc.Calculate(request);

        Assert.False(result.Success);
        Assert.False(rebateRepo.WasStoreCalled);
    }

    // --- Test helpers (simple fake repositories) ---
    private sealed class FakeRebateRepository : IRebateRepository
    {
        private readonly Dictionary<string, Rebate> _map;

        public FakeRebateRepository(Dictionary<string, Rebate> map) => _map = map;

        public bool WasStoreCalled { get; private set; }
        public decimal StoredAmount { get; private set; }
        public Rebate? StoredRebate { get; private set; }

        public Task<Rebate> GetRebate(string rebateIdentifier) =>
            Task.FromResult(_map.TryGetValue(rebateIdentifier, out var r) ? r : null!);

        public Task StoreCalculationResult(Rebate rebate, decimal amount)
        {
            WasStoreCalled = true;
            StoredAmount = amount;
            StoredRebate = rebate;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeProductRepository : IProductRepository
    {
        private readonly Dictionary<string, Product> _map;

        public FakeProductRepository(Dictionary<string, Product> map) => _map = map;

        public Task<Product> GetProduct(string productIdentifier) =>
            Task.FromResult(_map.TryGetValue(productIdentifier, out var p) ? p : null!);
    }
}
