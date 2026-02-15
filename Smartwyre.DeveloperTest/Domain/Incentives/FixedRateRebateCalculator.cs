using Smartwyre.DeveloperTest.Domain.Incentives.Interfaces;
using Smartwyre.DeveloperTest.Domain.Types;

namespace Smartwyre.DeveloperTest.Domain.Incentives
{
    public sealed class FixedRateRebateCalculator : IIncentiveCalculator
    {
        public IncentiveType SupportedIncentive => IncentiveType.FixedRateRebate;

        public CalculateRebateResult Calculate(Rebate rebate, Product product, CalculateRebateRequest request)
        {
            var result = new CalculateRebateResult();
            if (rebate is null || product is null)
            {
                result.Success = false;
                return result;
            }

            if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedRateRebate))
            {
                result.Success = false;
                return result;
            }

            if (rebate.Percentage == 0 || product.Price == 0 || request.Volume == 0)
            {
                result.Success = false;
                return result;
            }

            result.Amount += product.Price * rebate.Percentage * request.Volume;
            result.Success = true;
            return result;
        }
    }
}
