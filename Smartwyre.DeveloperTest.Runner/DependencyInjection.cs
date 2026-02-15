using Microsoft.Extensions.DependencyInjection;
using Smartwyre.DeveloperTest.Data.Interfaces;
using Smartwyre.DeveloperTest.Domain.Incentives;
using Smartwyre.DeveloperTest.Domain.Incentives.Interfaces;
using Smartwyre.DeveloperTest.Runner;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Services.Interfaces;

namespace Smartwyre.DeveloperTest.Runner;

public static class DependencyInjection
{
    public static IServiceCollection AddRunnerServices(this IServiceCollection services)
    {
        // Runner-only in-memory repositories
        services.AddSingleton<IRebateRepository, InMemoryRebateRepository>();
        services.AddSingleton<IProductRepository, InMemoryProductRepository>();

        // Register incentive calculators (add new calculators here)
        services.AddTransient<IIncentiveCalculator, FixedCashAmountCalculator>();
        services.AddTransient<IIncentiveCalculator, FixedRateRebateCalculator>();
        services.AddTransient<IIncentiveCalculator, AmountPerUomCalculator>();

        // Register service
        services.AddTransient<IRebateService, RebateService>();

        return services;
    }
}