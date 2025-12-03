using Bogus;
using Hymma.Lm.EndUser.Models;

namespace Hymma.Lm.EndUser.Test.Data
{
    public enum ProductType
    {
        NoFeatures = 0,
        OneFeature = 1,
        ManyFeatures = 2
    }
    public class Computers
    {
        public static ComputerModel FromRandom()
        {
            var computer = new Faker<ComputerModel>()
                .RuleFor(c => c.Name, f => f.Internet.DomainName())
                .RuleFor(c => c.MacAddress, f => f.Random.Uuid().ToString())
                .Generate(1)
                .First();
            return computer;
        }
    }
}
