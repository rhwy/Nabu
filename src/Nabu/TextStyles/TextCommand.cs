using System.Collections.Generic;
using System.Linq;

namespace Nabu.TextStyling
{
    public struct TextCommand
    {
        public List<string> Commands {get;set;}
        public string Content {get;set;}
        public bool IsNormal => !Commands.Any();
    }


}
