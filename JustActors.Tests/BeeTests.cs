using System.Threading.Tasks;
using JustActors.Tests.Actors;
using Xunit;

namespace JustActors.Tests
{
    public class BeeTests
    {
        private readonly IncrementBee _incrementor;
        private readonly SummatorBee _summator;

        public BeeTests()
        {
            _incrementor = new IncrementBee();
            _summator = new SummatorBee();
        }


        [Fact]
        public async Task Use_Incrementor()
        {
            var count = 1_000_000;
            for (var i = 0; i < count; i++)
            {
                _incrementor.Post(Unit.Value);
            }
            
            await _incrementor.WaitEmptyMailBox();
            Assert.Equal(count, _incrementor.GetState());
        }
        
        
        [Fact]
        public async Task Use_Summator()
        {
            var sum = await _summator.Sum(5, 5);

            Assert.Equal(10, sum);
        }
    }
}