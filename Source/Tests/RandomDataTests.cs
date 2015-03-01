using Xunit;

namespace Exceptionless.Tests
{
    public class RandomDataTests
    {
        [Fact]
        public void CanGenerateRandomData() {
            int value = RandomData.GetInt(1, 5);
            Assert.InRange(value, 1, 5);

            value = _numbers.Random();
            Assert.InRange(value, 1, 3);
        }

        private int[] _numbers = new[] {1, 2, 3};
    }
}
