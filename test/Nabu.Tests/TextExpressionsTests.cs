using NFluent;
using Xunit;
using System.Linq;
using System.Collections.Generic;

namespace Nabu.Tests
{
    public class TextStyleAdaptersTests
    {
        [Fact] public void 
        should_get_raw_text_with_inline_raw_adapter()
        {
            var text = TextStyleParser.Print(
                "#red`hello`",
                c => string.Join("",c.Select(x =>x.Content)));
            Check.That(text).IsEqualTo("hello");
        }

        [Fact] public void 
        should_get_raw_text_with_predefined_raw_adapter()
        {
            var text = TextStyleParser.Print(
                "#red`hello`", TextStyleAdapters.RawText);
            Check.That(text).IsEqualTo("hello");
        }

        class CustomRawAdapter : ITextStyleAdapter
        {
            public string Adapt(IEnumerable<TextCommand> expressions)
            {
                return string.Join("",expressions.Select(x =>x.Content));
            }
        }
        [Fact] public void 
        should_get_raw_text_with_custom_raw_adapter_interface()
        {
            var text = TextStyleParser.Print(
                "#red`hello`", new CustomRawAdapter());
            Check.That(text).IsEqualTo("hello");
        }


        [Theory] 
        [InlineData("#red`hello`","<span style=\"color:red\">hello</span>")] 
        [InlineData("#red#bold`hello`","<span style=\"color:red;font-weight:bold\">hello</span>")] public void 
        should_get_html_with_predefined_html_adapter(string source, string expectedHtml)
        {
            var text = TextStyleParser.Print(source, TextStyleAdapters.HtmlBasic);
            Check.That(text).IsEqualTo(expectedHtml);
        }

        [Theory] 
        [InlineData("#plop`hello`","hello")] 
        [InlineData("#green#glop`hello`","<span style=\"color:green\">hello</span>")] public void 
        should_get_clean_html_with_predefined_html_adapter_when_style_not_known(string source, string expectedHtml)
        {
            var text = TextStyleParser.Print(source, TextStyleAdapters.HtmlBasic);
            Check.That(text).IsEqualTo(expectedHtml);
        }
         
    }

    public class ParseTextExpressionsTests
    {
        [Fact] public void 
        should_parse_regular_text_as_normal()
        {
            var textCommand = TextStyleParser.ParseContent("hello").ToArray();
            Check.That(textCommand).HasSize(1);
            Check.That(textCommand[0].Content).IsEqualTo("hello");
            Check.That(textCommand[0].IsNormal).IsTrue();
            Check.That(textCommand[0].Commands).IsEmpty();
        }

        [Fact] public void 
        should_parse_one_command()
        {
            var textCommand = TextStyleParser.ParseContent("#red`hello`").ToArray();
            Check.That(textCommand).HasSize(1);
            Check.That(textCommand[0].Content).IsEqualTo("hello");
            Check.That(textCommand[0].IsNormal).IsFalse();
            Check.That(textCommand[0].Commands).ContainsExactly("red");
        }

        [Fact] public void 
        should_parse_text_with_multiple_commands_in_the_same_place()
        {
            var textCommand = TextStyleParser.ParseContent("#red#bold`hello`").ToArray();
            Check.That(textCommand).HasSize(1);
            Check.That(textCommand[0].Content).IsEqualTo("hello");
            Check.That(textCommand[0].IsNormal).IsFalse();
            Check.That(textCommand[0].Commands).ContainsExactly("red","bold");
        }

        [Fact] public void 
        should_parse_text_with_commandExpression_and_regular_text()
        {
            var textCommand = TextStyleParser.ParseContent("#red#bold`hello` world").ToArray();
            Check.That(textCommand).HasSize(2);

            Check.That(textCommand[0].Content).IsEqualTo("hello");
            Check.That(textCommand[0].IsNormal).IsFalse();
            Check.That(textCommand[0].Commands).ContainsExactly("red","bold");

            Check.That(textCommand[1].Content).IsEqualTo(" world");
            Check.That(textCommand[1].IsNormal).IsTrue();
            Check.That(textCommand[1].Commands).IsEmpty();
        }

        [Fact]
        public void should_parse_text_with_userStyle_command_and_style()
        {
            var textCommand = TextStyleParser.ParseContent("#warning`hello`",@"@warning:orange;").ToArray();
            Check.That(textCommand).HasSize(1);

            Check.That(textCommand[0].Content).IsEqualTo("hello");
            Check.That(textCommand[0].IsNormal).IsFalse();
            Check.That(textCommand[0].Commands).ContainsExactly("orange");
        }

        [Fact]
        public void should_parse_text_with_multiple_commands_multiple_styles()
        {
            var styles = @"
            @warning:orange,bold;
            @error:red,underline;";

            var content =  @"
            #10 #bold`machine1` is #green`working`
            #20 #bold`serviceX` send us a #warning`warning`";

            var commands = TextStyleParser.ParseContent(content,styles).ToArray();
            Check.That(commands).HasSize(8);

            //for easier testing, let's take the last element "#warning`warning`":
            Check.That(commands[7].Content).IsEqualTo("warning");
            Check.That(commands[7].IsNormal).IsFalse();
            Check.That(commands[7].Commands).ContainsExactly("orange","bold");
        }
    }
}

