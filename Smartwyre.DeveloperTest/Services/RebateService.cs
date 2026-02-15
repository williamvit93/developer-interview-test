using Smartwyre.DeveloperTest.Data.Interfaces;
using Smartwyre.DeveloperTest.Domain.Incentives.Interfaces;
using Smartwyre.DeveloperTest.Domain.Types;
using Smartwyre.DeveloperTest.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smartwyre.DeveloperTest.Services;

public sealed class RebateService : IRebateService
{
    private readonly IRebateRepository _rebateRepository;
    private readonly IProductRepository _productRepository;
    private readonly IEnumerable<IIncentiveCalculator> _calculators;
    public RebateService(IRebateRepository rebateRepository,
        IProductRepository productRepository,
        IEnumerable<IIncentiveCalculator> calculators)
    {
        _rebateRepository = rebateRepository;
        _productRepository = productRepository;
        _calculators = calculators;
    }

    public async Task<CalculateRebateResult> Calculate(CalculateRebateRequest request)
    {
        Rebate rebate = await _rebateRepository.GetRebate(request.RebateIdentifier);
        Product product = await _productRepository.GetProduct(request.ProductIdentifier);

        if (rebate is null || product is null)
            return new CalculateRebateResult { Success = false };

        var calculator = _calculators.FirstOrDefault(c => c.SupportedIncentive == rebate.Incentive);

        if (calculator is null)
            return new CalculateRebateResult { Success = false };

        var result = calculator.Calculate(rebate, product, request);

        if (result.Success)
            await _rebateRepository.StoreCalculationResult(rebate, result.Amount);

        return result;
    }
}
