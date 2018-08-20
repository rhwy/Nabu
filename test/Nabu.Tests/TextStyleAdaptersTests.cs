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
            [InlineData("#red`hello`","<span style=\"color:rgb(194,54,33)\">hello</span>")] 
            [InlineData("#red#bold`hello`","<span style=\"color:rgb(194,54,33);font-weight:bold\">hello</span>")] public void 
            should_get_html_with_predefined_html_adapter(string source, string expectedHtml)
            {
                var text = TextStyleParser.Print(source, TextStyleAdapters.HtmlBasic);
                Check.That(text).IsEqualTo(expectedHtml);
            }

            [Theory] 
            [InlineData("#plop`hello`","hello")] 
            [InlineData("#green#glop`hello`","<span style=\"color:rgb(37,188,36)\">hello</span>")] public void 
            should_get_clean_html_with_predefined_html_adapter_when_style_not_known(string source, string expectedHtml)
            {
                var text = TextStyleParser.Print(source, TextStyleAdapters.HtmlBasic);
                Check.That(text).IsEqualTo(expectedHtml);
            }
        }

        public class ToAnsiConsoleText
        {
            [Theory] 
            [InlineData("#green`hello`","\u001b[38;2;37;188;36mhello\u001b[39m")] 
            [InlineData("#red#bold`hello`","\u001b[38;2;194;54;33;1mhello\u001b[39;22m")] public void 
            should_get_ansi_string_with_predefined_ansi_adapter(string source, string expectedAnsiCode)
            {
                var text = TextStyleParser.Print(source, TextStyleAdapters.AnsiConsole);
                Check.That(text).IsEqualTo(expectedAnsiCode);
            }

            [Theory] 
            [InlineData("#green`hello`","\u001b[38;2;37;188;36mhello\u001b[39m")] 
            [InlineData("#red#bold`hello`","\u001b[38;2;194;54;33;1mhello\u001b[39;22m")] public void 
            can_generate_printer_for_reuse_with_defaults(string source, string expectedAnsiCode)
            {
                var printerDefaultConsole = TextStyleGenerator.GeneratePrinter();

                var text = printerDefaultConsole(source);
                Check.That(text).IsEqualTo(expectedAnsiCode);
            }

            [Theory] 
            [InlineData("#green`hello`","hello")] 
            [InlineData("#red#bold`hello`","hello")] public void 
            can_generate_printer_for_reuse_with_custom_adapter(string source, string expectedAnsiCode)
            {
                var adapter = new RawTextAdapter();
                var printerDefaultConsole = TextStyleGenerator.GeneratePrinter(adapter);

                var text = printerDefaultConsole(source);
                Check.That(text).IsEqualTo(expectedAnsiCode);
            }

            [Theory] 
            [InlineData("#green`hello`","<span style=\"color:rgb(166,226,45)\">hello</span>")] 
            [InlineData("#red#bold`hello`","<span style=\"color:rgb(253,66,133);font-weight:bold\">hello</span>")] public void 
            can_generate_printer_for_reuse_with_custom_adapter_and_palette(string source, string expectedAnsiCode)
            {
                var palette = new ColorPaletteMonokai();
                var adapter = new BasicHtmlAdapter(palette);
                var printerDefaultConsole = TextStyleGenerator.GeneratePrinter(adapter);

                var text = printerDefaultConsole(source);
                Check.That(text).IsEqualTo(expectedAnsiCode);
            }//@warning:orange,bold;

            [Theory] 
            [InlineData("#ok`hello`","@ok:green;","<span style=\"color:rgb(166,226,45)\">hello</span>")] 
            [InlineData("#error`hello`","@error:red,bold;","<span style=\"color:rgb(253,66,133);font-weight:bold\">hello</span>")] public void 
            can_generate_printer_for_reuse_with_custom_adapter_palette_and_style(
                string source, string styles,string expectedAnsiCode)
            {
                var palette = new ColorPaletteMonokai();
                var adapter = new BasicHtmlAdapter(palette);
                var printerDefaultConsole = TextStyleGenerator.GeneratePrinter(adapter,styles);

                var text = printerDefaultConsole(source);
                Check.That(text).IsEqualTo(expectedAnsiCode);
            }

            [Fact] public void
            can_create_reusable_console_style_generator()
            {
                var red = ConsoleStyles.Build("#red#bold");
                string sut = red("hello");
                Check.That(sut).IsEqualTo("\u001b[38;2;194;54;33;1mhello\u001b[39;22m");
            }

            [Fact] public void
            can_create_reusable_console_style_generator_with_palette()
            {
                var red = ConsoleStyles.Build("#red#bold#underline", new ColorPaletteMonokai());
                string sut = red("hello") + " world";
                Check.That(sut).IsEqualTo("\u001b[38;2;253;66;133;1;4mhello\u001b[39;22;24m world");
            }
        }

        public class UseColorPalette
        {
            [Fact] public void
            should_use_color_model()
            {
                var color = new Color(80);
                var sut = color.ToStringList();
                Check.That(sut).IsEqualTo("80,0,0");
                sut = color.ToStringList(separator:";");
                Check.That(sut).IsEqualTo("80;0;0");
            }

            [Theory] 
            [InlineData("#green`hello`","\u001b[38;2;166;226;45mhello\u001b[39m")] 
            [InlineData("#red#bold`hello`","\u001b[38;2;253;66;133;1mhello\u001b[39;22m")] public void 
            can_use_an_alternate_color_palette_with_console_adapter(string source, string expectedAnsiCode)
            {
                var customPalette = new ColorPaletteMonokai();
                var ansiConsoleAdapter = new AnsiConsoleAdapter(customPalette);
                var text = TextStyleParser.Print(source, ansiConsoleAdapter);
                Check.That(text).IsEqualTo(expectedAnsiCode);
            }

            [Theory] 
            [InlineData("#red`hello`","<span style=\"color:rgb(253,66,133)\">hello</span>")] 
            [InlineData("#red#bold`hello`","<span style=\"color:rgb(253,66,133);font-weight:bold\">hello</span>")] public void 
            can_use_an_alternate_color_palette_with_html_adapter(string source, string expectedHtml)
            {
                var customPalette = new ColorPaletteMonokai();
                var htmlAdapter = new BasicHtmlAdapter(customPalette);
                var text = TextStyleParser.Print(source, htmlAdapter);
                Check.That(text).IsEqualTo(expectedHtml);
            }


        }
    }
}

