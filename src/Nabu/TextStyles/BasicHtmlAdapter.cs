using System.Collections.Generic;
using System.Linq;
using Sprache;

namespace Nabu.TextStyling
{
    public class BasicHtmlAdapter : ITextStyleAdapter
    {
        public IColorPalette Palette {get;set;}

        public BasicHtmlAdapter(IColorPalette palette = null)
        {
            Palette = palette ?? new ColorPaletteTerminal();
        }
        public string Adapt(IEnumerable<TextCommand> expressions)
        => toHtmlText(expressions);
        
        string getHtmlStyle (string command)
        {
            switch(command){
                //foreground
                case "black": return $"color:rgb({Palette.Black})";
                case "red": return $"color:rgb({Palette.Red})";
                case "green": return $"color:rgb({Palette.Green})";
                case "yellow": case "orange" : return $"color:rgb({Palette.Yellow})";
                case "blue": return $"color:rgb({Palette.Blue})";
                case "magenta": case "pink": return $"color:rgb({Palette.Magenta})";
                case "cyan" : return $"color:rgb({Palette.Cyan})";
                case "white" : return $"color:rgb({Palette.White})";

                //foreground
                case "bgblack": return $"background-color:rgb({Palette.Black})";
                case "bgred": return $"background-color:rgb({Palette.Red})";
                case "bggreen": return $"background-color:rgb({Palette.Green})";
                case "bgyellow": case "bgorange" : return $"background-color:rgb({Palette.Yellow})";
                case "bgblue": return $"background-color:rgb({Palette.Blue})";
                case "bgmagenta": case "bgpink": return $"background-color:rgb({Palette.Magenta})";
                case "bgcyan" : return $"background-color:rgb({Palette.Cyan})";
                case "bgwhite" : return $"background-color:rgb({Palette.White})";

                //effect
                case "bold": return "font-weight:bold";
                case "italic": return "font-style: italic;";
                case "underline": return "text-decoration:underline";
                default: return null;
            }
        }
        string toHtmlText(IEnumerable<TextCommand> c) => c.Select(x=> {
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

        
    }


}
