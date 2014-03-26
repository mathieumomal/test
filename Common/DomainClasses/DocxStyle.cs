using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Novacode;

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

    }
}
