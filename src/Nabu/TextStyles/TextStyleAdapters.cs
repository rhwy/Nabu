using System;
using System.Collections.Generic;
using System.Linq;
using Sprache;

namespace Nabu.TextStyling
{
    public static class TextStyleAdapters
    {
        public static Func<IEnumerable<TextCommand>,string> RawText {get;} =
        (c) => c.
            Select      (  x    => x.Content ).
            Aggregate   ( (a,b) => a+b       );


        static Func<string,string> getHtmlStyle = command => {
            switch(command){
                case "black": return "color:rgb(0,0,0)";
                case "red": return "color:rgb(170,0,0)";
                case "green": return "color:rgb(0,170,0)";
                case "yellow": case "orange" : return "color:rgb(0,170,0)";
                case "blue": return "color:rgb(0,0,170)";
                case "magenta": case "pink": return "color:rgb(170,0,170)";
                case "cyan" : return "color:rgb(0,170,170)";
                case "white" : return "color:rgb(170,170,170)";
                case "bold": return "font-weight:bold";
                case "italic": return "font-style: italic;";
                case "underline": return "text-decoration:underline";
                default: return null;
            }
        };

        static Func<string,(int start,int stop)> getConsoleCodes = command => {
            switch(command){
                case "black": return (30,39);
                case "red": return (31,39);
                case "green": return (32,39);
                case "orange": case "yellow": return (32,39);
                case "blue": return (34,39);
                case "magenta": return (35,39);
                case "cyan": return (36,39);
                case "white": return (37,39);
                case "bold": return (1,21);
                case "underline": return (4,24);
                case "italic": return (3,23);
                default: return (-1,39);
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

        static Func<IEnumerable<TextCommand>,string> toConsoleText = (c) => c.Select(x=> {
            if(x.IsNormal) return x.Content;
            var styles = x.Commands
                        .Select(m=>getConsoleCodes(m))
                        .Where( s => s.start != -1);

            if(styles.Any())
            {
                return "\u001b["
                        + styles.
                            Select(s=> s.start.ToString()).
                            Aggregate((acc,current)=>$"{acc};{current}")
                        + "m"
                        + x.Content
                        + "\u001b["
                        + styles.
                            Select(s=> s.stop.ToString()).
                            Aggregate((acc,current)=>$"{acc};{current}")
                        + "m";
            } else {
                return x.Content;
            }          
        }).Aggregate((a,b)=>a+b);
        public static Func<IEnumerable<TextCommand>,string> HtmlBasic {get;} =
        toHtmlText;

        public static Func<IEnumerable<TextCommand>,string> AnsiConsole {get;} =
        toConsoleText;
    }


}
