using Smartwyre.DeveloperTest.Domain.Incentives.Interfaces;
using Smartwyre.DeveloperTest.Domain.Types;

namespace Smartwyre.DeveloperTest.Domain.Incentives
{
    public sealed class FixedCashAmountCalculator : IIncentiveCalculator
    {
        public IncentiveType SupportedIncentive => IncentiveType.FixedCashAmount;

        public CalculateRebateResult Calculate(Rebate rebate, Product product, CalculateRebateRequest request)
        {
            var result = new CalculateRebateResult();

            if (rebate is null || product is null)
            {
                result.Success = false;
                return result;
            }

            if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedCashAmount))
            {
                result.Success = false;
                return result;
            }

            if (rebate.Amount <= 0)
            {
                result.Success = false;
                return result;
            }

            result.Success = true;
            result.Amount = rebate.Amount;
            return result;
        }
    }
}
