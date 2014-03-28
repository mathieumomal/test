using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain = Etsi.Ultimate.DomainClasses;
using System.Drawing;

namespace Etsi.Ultimate.DomainClasses
{
    public class DocxStylePool
    {
        public enum STYLES_KEY { BLACK_WHITE = 1, BLACK_GREEN = 2, BLACK_GRAY = 3, BOLD_BLACK_WHITE = 4, BOLD_BLACK_GREEN = 5, BOLD_BLACK_GRAY = 6, BLUE_WHITE = 7, BLUE_GREEN = 8, BLUE_GRAY = 9, RED_WHITE = 10, RED_GREEN = 11, RED_GRAY = 12 }
        
        private static Dictionary<STYLES_KEY, DocxStyle> docxStyles = new Dictionary<STYLES_KEY, DocxStyle>()
        {
            
            {STYLES_KEY.BLACK_WHITE, new DocxStyle(Color.White, Color.Black, false)},
            {STYLES_KEY.BLACK_GREEN, new DocxStyle(Color.FromArgb(204, 255, 204), Color.Black, false)},
            {STYLES_KEY.BLACK_GRAY, new DocxStyle(Color.FromArgb(227, 227, 227), Color.Black, false)},

            {STYLES_KEY.BOLD_BLACK_WHITE, new DocxStyle(Color.White, Color.Black, true)},
            {STYLES_KEY.BOLD_BLACK_GREEN, new DocxStyle(Color.FromArgb(204, 255, 204), Color.Black, true)},
            {STYLES_KEY.BOLD_BLACK_GRAY, new DocxStyle(Color.FromArgb(227, 227, 227), Color.Black, true)},            

            {STYLES_KEY.BLUE_WHITE, new DocxStyle(Color.White, Color.Blue, true)},
            {STYLES_KEY.BLUE_GREEN, new DocxStyle(Color.FromArgb(204, 255, 204), Color.Blue, true)},
            {STYLES_KEY.BLUE_GRAY, new DocxStyle(Color.FromArgb(227, 227, 227), Color.Blue, true)},

            {STYLES_KEY.RED_WHITE, new DocxStyle(Color.White, Color.Red, true)},
            {STYLES_KEY.RED_GREEN, new DocxStyle(Color.FromArgb(204, 255, 204), Color.Red, true)},
            {STYLES_KEY.RED_GRAY, new DocxStyle(Color.FromArgb(227, 227, 227), Color.Red, true)}
        };

        public static DocxStyle GetDocxStyle(int value)
        {
            return docxStyles[((STYLES_KEY)value)];
        }
    }
}
