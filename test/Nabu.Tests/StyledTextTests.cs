using NFluent;
using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace Nabu.Tests
{
    public class StyledTextTests
    {
        [Fact]
        public void should_parse_one_element_of_style()
        {
            var warningStyle = TextStyleParser.ParseStyles("@warning:orange,bold;");
            Check.That(warningStyle.Styles.First()).HasFieldsWithSameValues(new { Name="warning",Styles=new[]{"orange","bold"}});
            Check.That(warningStyle.GetStyledText("hello","warning")).IsEqualTo("#orange#bold`hello`");
        }

        [Fact]
        public void should_parse_styles_with_multiples_elements()
        {
            var multipleStyles = TextStyleParser.ParseStyles(@"
            @warning:orange,bold;
            @error:red,underline;");
            Check.That(multipleStyles.Styles).HasSize(2);
            Check.That(multipleStyles.GetStyledText("hello","error")).IsEqualTo("#red#underline`hello`");
            Check.That(multipleStyles.GetStyledText("hello","not_an_existing_style")).IsEqualTo("hello");
        }
    }
}

