using Smartwyre.DeveloperTest.Domain.Incentives.Interfaces;
using Smartwyre.DeveloperTest.Domain.Types;

namespace Smartwyre.DeveloperTest.Domain.Incentives
{
    public sealed class AmountPerUomCalculator : IIncentiveCalculator
    {
        public IncentiveType SupportedIncentive => IncentiveType.AmountPerUom;

        public CalculateRebateResult Calculate(Rebate rebate, Product product, CalculateRebateRequest request)
        {
            var result = new CalculateRebateResult();

            if (rebate == null || product == null)
            {
                result.Success = false;
                return result;
            }

            if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.AmountPerUom))
            {
                result.Success = false;
                return result;
            }

            if (rebate.Amount == 0 || request.Volume == 0)
            {
                result.Success = false;
                return result;
            }

            result.Amount += rebate.Amount * request.Volume;
            result.Success = true;
            return result;
        }
    }
}
