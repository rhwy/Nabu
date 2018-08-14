using System.Linq;

namespace Nabu
{
    public static class StringExtensions
    {
        public static string Replicate(this string current, int times)
        => string.Join(
            string.Empty,
            Enumerable.Repeat(current,times));
        

        public static string Center(this string current, int width)
        => current
                .PadLeft((current.Length+width)/2)
                .PadRight(width);
        
    }
}
