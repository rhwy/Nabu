namespace Nabu.TextStyling
{
    public class ColorPaletteTerminal : IColorPalette
    {
        public Color Black {get;} = Color.From(0,0,0);
        public Color Red {get;} = Color.From(194,54,33);
        public Color Green {get;} = Color.From(37,188,36);
        public Color Yellow {get;} = Color.From(173,173,39);
        public Color Blue {get;} = Color.From(73,46,225);
        public Color Magenta {get;} = Color.From(211,56,211);
        public Color Cyan {get;} = Color.From(51,187,200);
        public Color White {get;} = Color.From(203,204,205);
        
    }


}
