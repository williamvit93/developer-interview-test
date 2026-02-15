using Smartwyre.DeveloperTest.Domain.Types;
using System.Threading.Tasks;

namespace Smartwyre.DeveloperTest.Services.Interfaces;

public interface IRebateService
{
    Task<CalculateRebateResult> Calculate(CalculateRebateRequest request);
}
