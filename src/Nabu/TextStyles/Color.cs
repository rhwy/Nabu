namespace Nabu.TextStyling
{
    public struct Color
    {
        public byte Red {get;}
        public byte Green {get;}
        public byte Blue {get;}
        public Color(byte red = 0, byte green = 0, byte blue = 0)
        {
            Red = red; Green = green; Blue = blue;
        }

        public static Color From(byte red, byte green, byte blue)
        => new Color(red,green,blue); 

        public string ToStringList()
        => $"{Red},{Green},{Blue}";
    }


}
