using JustActors.Tests.Actors;
using Xunit;

namespace JustActors.Tests
{
    public class BeeApiaryTests
    {
        private readonly BeeApiary _beeApiary;

        public BeeApiaryTests()
        {
            _beeApiary = new BeeApiary(new DefaultBeeResolver());
        }


        [Fact]
        public void Bee_should_be_resolved()
        {
            var actualBee = new IncrementBee();
            
            _beeApiary.RegisterBee("be_1", actualBee);
            
            var bee = _beeApiary.GetBee<IncrementBee>("be_1");
            
            Assert.NotNull(bee);
            Assert.Equal(actualBee, bee);
        }

        [Fact]
        public void Bee_should_be_created()
        {
            var bee = _beeApiary.GetBee<IncrementBee>("be_1");
            
            Assert.NotNull(bee);
        }
    }
}