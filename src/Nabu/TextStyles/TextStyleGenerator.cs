using System;

namespace Nabu.TextStyling
{
    public static class TextStyleGenerator
    {
        static ITextStyleAdapter defaultAdapter = new AnsiConsoleAdapter();
        public static Func<string,string> GeneratePrinter(ITextStyleAdapter adapter = null, string styles = null)
        => (x) => TextStyleParser.Print(x,adapter ?? defaultAdapter, styles);
        
    }


}
