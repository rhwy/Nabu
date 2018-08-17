using System;
using System.Collections.Generic;
using System.Linq;
using Sprache;

namespace Nabu.TextStyling
{
    public static class TextStyleParser
    {
        public static TextStyles ParseStyles(string styles)
        {
            return stylesParser.Parse(styles.Trim());
        }
        public static IEnumerable<TextCommand> ParseContent(string content, string styles = null)
        {
            string workcontent = string.IsNullOrEmpty(styles)
                ? content
                : ParseStyles(styles).UpdateStyles(content);

            return styledExpression.Parse(workcontent);
        }
        
        public static string Print(string content, Func<IEnumerable<TextCommand>,string> outputAdapter, string styles = null)
        => outputAdapter(ParseContent(content,styles));

        public static string Print(string content, ITextStyleAdapter outputAdapter, string styles = null)
        => outputAdapter.Adapt(ParseContent(content,styles));

        //Styles: 
        static Parser<IEnumerable<string>> comaList =
            Parse.DelimitedBy(
                Parse.LetterOrDigit.Many().Text(),
                Parse.Char(','));

        static Parser<TextNamedStyle> oneStyle = 
            from space in Parse.CharExcept('@').Many().Text()
            from start in Parse.Char('@')
            from name in Parse.LetterOrDigit.Many().Text()
            from sep in Parse.Char(':')
            from styles in comaList
            from stop in Parse.Char(';')
            select new TextNamedStyle(name,styles);

        static Parser<TextStyles> stylesParser = 
            from s in oneStyle.Many()
            select new TextStyles(s);

        //expression
        static Parser<string> internalText = 
            from entry in Parse.Char('`')
            from content in Parse.CharExcept(new[]{'`'}).Many().Text()
            from finish in Parse.Char('`')
            select content;

        static Parser<string> commandText = 
            from entry in Parse.Char('#')
            from content in Parse.CharExcept(new[]{'`','#',' '}).Many().Text()
            select content;

        static Parser<string> separator = 
            from c in Parse.Char('#').AtLeastOnce()
            from t in Parse.CharExcept(new[]{'`','#',' '}).Many().Text()
            from e in Parse.Char('#').Or(Parse.Char('`'))
            select "";

        static Parser<TextCommand> textExpression = 
            from command in commandText.AtLeastOnce()
            from content in internalText
            select new TextCommand{ Commands = command.ToList<string>(), Content=content};

        static Parser<TextCommand> regularText =
            from text in Parse.AnyChar.Except(separator).Many().Text()
            select new TextCommand{ Commands = new List<string>(), Content=text};

        static Parser<IEnumerable<TextCommand>> styledExpression = 
            (textExpression.Or(regularText).Or(textExpression)).Many();

    }


}
