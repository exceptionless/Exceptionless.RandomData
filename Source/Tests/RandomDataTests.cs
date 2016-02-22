using Xunit;

namespace Exceptionless.Tests {
    public class RandomDataTests {
        [Fact]
        public void RandomInt() {
            int value = RandomData.GetInt(1, 5);
            Assert.InRange(value, 1, 5);

            value = _numbers.Random();
            Assert.InRange(value, 1, 3);
        }

        [Fact]
        public void RandomDecimal() {
            decimal value = RandomData.GetDecimal(1, 5);
            Assert.InRange(value, 1, 5);
        }
        
        [Fact]
        public void GetEnumWithOneValueTest() {
            var result = RandomData.GetEnum<_days>();

            Assert.Equal<_days>(_days.Monday, result);
        }

        [Fact]
        public void GetSentencesTest() {
            var result = RandomData.GetSentence();

            Assert.False(string.IsNullOrEmpty(result));
        }

        private int[] _numbers = new[] { 1, 2, 3 };

        private enum _days {
            Monday
        }
    }
}