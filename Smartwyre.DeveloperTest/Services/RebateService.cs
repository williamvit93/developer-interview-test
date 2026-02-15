using Smartwyre.DeveloperTest.Data.Interfaces;
using Smartwyre.DeveloperTest.Services.Interfaces;
using Smartwyre.DeveloperTest.Types;
using System.Threading.Tasks;

namespace Smartwyre.DeveloperTest.Services;

public class RebateService : IRebateService
{
    private readonly IRebateRepository _rebateRepository;
    private readonly IProductRepository _productRepository;
    public RebateService(IRebateRepository rebateRepository, IProductRepository productRepository)
    {
        _rebateRepository = rebateRepository;
        _productRepository = productRepository;
    }

    public async Task<CalculateRebateResult> Calculate(CalculateRebateRequest request)
    {
        Rebate rebate = await _rebateRepository.GetRebate(request.RebateIdentifier);
        Product product = await _productRepository.GetProduct(request.ProductIdentifier);

        var result = new CalculateRebateResult();

        var rebateAmount = 0m;

        switch (rebate.Incentive)
        {
            case IncentiveType.FixedCashAmount:
                if (rebate == null)
                {
                    result.Success = false;
                }
                else if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedCashAmount))
                {
                    result.Success = false;
                }
                else if (rebate.Amount == 0)
                {
                    result.Success = false;
                }
                else
                {
                    rebateAmount = rebate.Amount;
                    result.Success = true;
                }
                break;

            case IncentiveType.FixedRateRebate:
                if (rebate == null)
                {
                    result.Success = false;
                }
                else if (product == null)
                {
                    result.Success = false;
                }
                else if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedRateRebate))
                {
                    result.Success = false;
                }
                else if (rebate.Percentage == 0 || product.Price == 0 || request.Volume == 0)
                {
                    result.Success = false;
                }
                else
                {
                    rebateAmount += product.Price * rebate.Percentage * request.Volume;
                    result.Success = true;
                }
                break;

            case IncentiveType.AmountPerUom:
                if (rebate == null)
                {
                    result.Success = false;
                }
                else if (product == null)
                {
                    result.Success = false;
                }
                else if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.AmountPerUom))
                {
                    result.Success = false;
                }
                else if (rebate.Amount == 0 || request.Volume == 0)
                {
                    result.Success = false;
                }
                else
                {
                    rebateAmount += rebate.Amount * request.Volume;
                    result.Success = true;
                }
                break;
        }

        if (result.Success)
        {
            await _rebateRepository.StoreCalculationResult(rebate, rebateAmount);
        }

        return result;
    }
}
