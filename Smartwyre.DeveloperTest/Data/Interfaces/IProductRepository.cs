using Smartwyre.DeveloperTest.Types;
using System.Threading.Tasks;

namespace Smartwyre.DeveloperTest.Data.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> GetProduct(string productIdentifier);
    }
}
