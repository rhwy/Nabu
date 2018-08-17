using System.Collections.Generic;

namespace Nabu.TextStyling
{
    public interface ITextStyleAdapter
    {
        string Adapt(IEnumerable<TextCommand> expressions);
    }


}
