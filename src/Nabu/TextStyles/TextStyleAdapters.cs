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


}
