using NFluent;
using Xunit;
using System.Linq;
using System.Collections.Generic;
using Nabu.TextStyling;

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
}

