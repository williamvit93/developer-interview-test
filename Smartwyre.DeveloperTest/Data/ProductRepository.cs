using Smartwyre.DeveloperTest.Data.Interfaces;
using Smartwyre.DeveloperTest.Types;
using System.Threading.Tasks;

namespace Smartwyre.DeveloperTest.Data;

public class ProductRepository : IProductRepository
{
    public Task<Product> GetProduct(string productIdentifier)
    {
        // Access database to retrieve account, code removed for brevity 
        return Task.FromResult(new Product());
    }
}
