using System;
using System.Collections.Generic;

namespace Nabu.TextStyling
{

    public static class TextStyleAdapters
    {
        public static IColorPalette Palette {get;set;} = new ColorPaletteTerminal();
        public static ITextStyleAdapter AnsiConsoleAdapter {get;} = new AnsiConsoleAdapter();
        public static ITextStyleAdapter BasicHtmlAdapter {get;} = new BasicHtmlAdapter();
        public static ITextStyleAdapter RawTextAdapter {get;} = new RawTextAdapter();
        
        public static Func<IEnumerable<TextCommand>,string> RawText {get;} =
        RawTextAdapter.Adapt;

        public static Func<IEnumerable<TextCommand>,string> HtmlBasic {get;} =
        BasicHtmlAdapter.Adapt;
        public static Func<IEnumerable<TextCommand>,string> AnsiConsole {get;} =
        AnsiConsoleAdapter.Adapt;
    }


}
