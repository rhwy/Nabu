using NFluent;
using Xunit;

namespace Nabu.Tests
{
    public class StringExtentionsTests
    {
        [Theory]
        [InlineData(10,"  hello   ")]
        [InlineData(15,"     hello     ")]
        public void string_can_be_centered(
            int width, string expected
        )
        {
            var content = "hello";
            var sut = content.Center(width);
            Check.That(sut.Length).IsEqualTo(width);
            Check.That(sut).IsEqualTo(expected);
        }

        [Theory]
        [InlineData("-",1,"-")]
        [InlineData("-",2,"--")]
        [InlineData("-",3,"---")]
        [InlineData("░",10,"░░░░░░░░░░")]
        public void string_can_be_replicated(
            string pattern, int length, string expected
        )
        {
            var sut = pattern.Replicate(length);
            Check.That(sut.Length).IsEqualTo(length);
            Check.That(sut).IsEqualTo(expected);
        }
    }
}
