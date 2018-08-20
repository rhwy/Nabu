using System.Collections.Generic;
using System.Linq;
using Sprache;

namespace Nabu.TextStyling
{
    public class AnsiConsoleAdapter : ITextStyleAdapter
    {
        public IColorPalette Palette {get;set;}

        public AnsiConsoleAdapter(IColorPalette palette = null)
        {
            Palette = palette ?? new ColorPaletteTerminal();
        }

        public string Adapt(IEnumerable<TextCommand> expressions)
        => toConsoleText(expressions);

        (int start,int stop) getBasicConsoleCodes (string command )
        {
            switch(command){
                case "black": return (30,39);
                case "red": return (31,39);
                case "green": return (32,39);
                case "orange": case "yellow": return (32,39);
                case "blue": return (34,39);
                case "magenta": return (35,39);
                case "cyan": return (36,39);
                case "white": return (37,39);
                case "bold": return (1,22);
                case "underline": return (4,24);
                case "italic": return (3,23);
                default: return (-1,39);
            }
        }

        (string start,int stop) getConsoleCodes (string command){
            switch(command){
                //foreground
                case "black": return ($"38;2;{Palette.Black.ToStringList(";")}",39);
                case "red": return ($"38;2;{Palette.Red.ToStringList(";")}",39);
                case "green": return ($"38;2;{Palette.Green.ToStringList(";")}",39);
                case "orange": case "yellow": return ($"38;2;{Palette.Yellow.ToStringList(";")}",39);
                case "blue": return ($"38;2;{Palette.Blue.ToStringList(";")}",39);
                case "magenta": return ($"38;2;{Palette.Magenta.ToStringList(";")}",39);
                case "cyan": return ($"38;2;{Palette.Cyan.ToStringList(";")}",39);
                case "white": return ($"38;2;{Palette.White.ToStringList(";")}",39);
                //background 
                case "bgblack": return ($"48;2;{Palette.Black.ToStringList(";")}",49);
                case "bgred": return ($"48;2;{Palette.Red.ToStringList(";")}",49);
                case "bggreen": return ($"48;2;{Palette.Green.ToStringList(";")}",49);
                case "bgorange": case "bgyellow": return ($"48;2;{Palette.Yellow.ToStringList(";")}",49);
                case "bgblue": return ($"48;2;{Palette.Blue.ToStringList(";")}",49);
                case "bgmagenta": return ($"48;2;{Palette.Magenta.ToStringList(";")}",49);
                case "bgcyan": return ($"48;2;{Palette.Cyan.ToStringList(";")}",49);
                case "bgwhite": return ($"48;2;{Palette.White.ToStringList(";")}",49);

                //effect
                case "bold": return ("1",22);
                case "underline": return ("4",24);
                case "italic": return ("3",0);
                default: return ("",39);
            }
        }

        string toConsoleText (IEnumerable<TextCommand> c) 
        => c.Select(x=> {
            if(x.IsNormal) return x.Content;
            var styles = x.Commands
                        .Select(m=>getConsoleCodes(m))
                        .Where( s => s.start != "");

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
    }


}
