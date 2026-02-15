using Smartwyre.DeveloperTest.Domain.Types;

namespace Smartwyre.DeveloperTest.Domain.Incentives.Interfaces
{
    public interface IIncentiveCalculator
    {
        IncentiveType SupportedIncentive { get; }
        CalculateRebateResult Calculate(Rebate rebate, Product product, CalculateRebateRequest request);
    }
}
