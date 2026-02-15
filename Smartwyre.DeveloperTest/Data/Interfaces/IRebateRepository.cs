using Smartwyre.DeveloperTest.Types;
using System.Threading.Tasks;

namespace Smartwyre.DeveloperTest.Data.Interfaces
{
    public interface IRebateRepository
    {
        Task<Rebate> GetRebate(string rebateIdentifier);
        Task StoreCalculationResult(Rebate account, decimal rebateAmount);
    }
}
