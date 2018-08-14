using System;
using System.Collections.Generic;
using System.Linq;
using Sprache;

namespace Nabu
{
    public struct TextNamedStyle
    {
        public string Name {get;}
        public IEnumerable<string> Styles {get;}
        public TextNamedStyle(string name, IEnumerable<string> styles)
        {
            Name = name; Styles = styles;
        }
        public string GetStyles()
        => Styles.Select(x=>$"#{x}").Aggregate((c,acc)=>c+acc);

        public string GetStyledText(string content)
        => $"{GetStyles()}`{content}`";

        public string UpdateStyle(string content)
        => content.Replace($"#{Name}",GetStyles()); 
    }

    public struct TextStyles
    {
        private Dictionary<string,TextNamedStyle> styles;
        public IEnumerable<TextNamedStyle> Styles => styles.Values;
        public TextStyles(IEnumerable<TextNamedStyle> styles)
        {
            this.styles = styles.ToDictionary(x=>x.Name,y=>y);
        }
        public string GetStyledText(string content,string styleName)
        {
            if(styles.ContainsKey(styleName))
            {
                return styles[styleName].GetStyledText(content);
            }
            return content;
        }
        public string UpdateStyles(string content)
        {
            foreach(var style in Styles)
            {
                content = style.UpdateStyle(content);
            } 
            return content;
        } 
    }

    public struct TextCommand
    {
        public List<string> Commands {get;set;}
        public string Content {get;set;}
        public bool IsNormal => !Commands.Any();
    }

    public interface ITextStyleAdapter
    {
        string Adapt(IEnumerable<TextCommand> expressions);
    }
    public static class TextStyleAdapters
    {
        public static Func<IEnumerable<TextCommand>,string> RawText {get;} =
        (c) => c.
            Select      (  x    => x.Content ).
            Aggregate   ( (a,b) => a+b       );


        static Func<string,string> getHtmlStyle = command => {
            switch(command){
                case "red": return "color:red";
                case "green": return "color:green";
                case "orange": case "yellow": return "color:orange";
                case "bold": return "font-weight:bold";
                case "underline": return "text-decoration:underline";
                default: return null;
            }
        };

        static Func<IEnumerable<TextCommand>,string> toHtmlText = (c) => c.Select(x=> {
            if(x.IsNormal) return x.Content;
            var styles = x.Commands
                        .Select(m=>getHtmlStyle(m))
                        .Where( s => s != null);
            if(styles.Any())
            {
            return "<span style=\""
                    + styles.Aggregate((a,b)=>a+";"+b)
                    + "\">"
                    + x.Content
                    + "</span>";
            } else {
                return x.Content;
            }          
        }).Aggregate((a,b)=>a.Replace("\n","<br>")+b);
        public static Func<IEnumerable<TextCommand>,string> HtmlBasic {get;} =
        toHtmlText;

    }
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
