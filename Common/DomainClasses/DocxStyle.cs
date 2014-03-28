using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Etsi.Ultimate.DomainClasses
{
    public class DocxStyle
    {
        public Color BgColor {get; set;}
        public Color FontColor {get; set;}
        public bool IsBold {get; set;}

        public DocxStyle(){
        }

        public DocxStyle( Color bgColor, Color fontColor, bool isBold){
            BgColor= bgColor;
            FontColor= fontColor;
            IsBold= isBold;            
        }

        public String GetFontColorHex()
        {
            return FontColor.R.ToString("X2") + FontColor.G.ToString("X2") + FontColor.B.ToString("X2");
        }

        public String GetBgColorHex()
        {
            return BgColor.R.ToString("X2") + BgColor.G.ToString("X2") + BgColor.B.ToString("X2");
        }

    }
}
