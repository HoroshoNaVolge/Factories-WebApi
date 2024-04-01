using Microsoft.IdentityModel.Tokens;

namespace Factories.WebApi.BLL.Services
{
    public interface IRandomService
    {
        double NextDouble();
    }
    public class RandomService : IRandomService
    {
        private readonly Random random = new();
        public double NextDouble() => random.NextDouble();
    }
}
