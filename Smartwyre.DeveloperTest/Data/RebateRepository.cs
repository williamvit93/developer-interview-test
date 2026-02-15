using Smartwyre.DeveloperTest.Data.Interfaces;
using Smartwyre.DeveloperTest.Types;
using System.Threading.Tasks;

namespace Smartwyre.DeveloperTest.Data;

public class RebateRepository : IRebateRepository
{
    public Task<Rebate> GetRebate(string rebateIdentifier)
    {
        // Access database to retrieve account, code removed for brevity 
        return Task.FromResult(new Rebate());
    }

    public Task StoreCalculationResult(Rebate account, decimal rebateAmount)
    {
        // Update account in database, code removed for brevity
        return Task.CompletedTask;
    }
}
