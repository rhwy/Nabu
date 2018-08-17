using System.Collections.Generic;
using System.Linq;

namespace Nabu.TextStyling
{
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


}
