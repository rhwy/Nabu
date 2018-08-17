using NFluent;
using Xunit;
using System.Linq;
using System.Collections.Generic;
using Nabu.TextStyling;

namespace Nabu.Tests
{
    public class TextStyleAdaptersTests
    {
        public class ToRawText 
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
        }

        public class ToHtmlText
        {
            [Theory] 
            [InlineData("#red`hello`","<span style=\"color:rgb(170,0,0)\">hello</span>")] 
            [InlineData("#red#bold`hello`","<span style=\"color:rgb(170,0,0);font-weight:bold\">hello</span>")] public void 
            should_get_html_with_predefined_html_adapter(string source, string expectedHtml)
            {
                var text = TextStyleParser.Print(source, TextStyleAdapters.HtmlBasic);
                Check.That(text).IsEqualTo(expectedHtml);
            }

            [Theory] 
            [InlineData("#plop`hello`","hello")] 
            [InlineData("#green#glop`hello`","<span style=\"color:rgb(0,170,0)\">hello</span>")] public void 
            should_get_clean_html_with_predefined_html_adapter_when_style_not_known(string source, string expectedHtml)
            {
                var text = TextStyleParser.Print(source, TextStyleAdapters.HtmlBasic);
                Check.That(text).IsEqualTo(expectedHtml);
            }
        }

        public class ToAnsiConsoleText
        {
            [Theory] 
            [InlineData("#green`hello`","\u001b[32mhello\u001b[39m")] 
            [InlineData("#red#bold`hello`","\u001b[31;1mhello\u001b[39;21m")] public void 
            should_get_ansi_string_with_predefined_ansi_adapter(string source, string expectedAnsiCode)
            {
                var text = TextStyleParser.Print(source, TextStyleAdapters.AnsiConsole);
                Check.That(text).IsEqualTo(expectedAnsiCode);
            }

        }
    }
}

