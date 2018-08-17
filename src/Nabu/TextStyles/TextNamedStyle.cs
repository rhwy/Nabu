using System.Collections.Generic;
using System.Linq;

namespace Nabu.TextStyling
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


}
