using System.Collections.Generic;
using System.Linq;
using Sprache;

namespace Nabu.TextStyling
{
    public class RawTextAdapter : ITextStyleAdapter
    {
        public string Adapt(IEnumerable<TextCommand> expressions)
        => expressions.
            Select      (  x    => x.Content ).
            Aggregate   ( (a,b) => a+b       );
    }


}
