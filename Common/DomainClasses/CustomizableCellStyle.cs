using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.SS.UserModel;

namespace Etsi.Ultimate.DomainClasses
{
    public class CustomizableCellStyle : ICellStyle
    {
        public HorizontalAlignment Alignment
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public BorderStyle BorderBottom
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public BorderStyle BorderLeft
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public BorderStyle BorderRight
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public BorderStyle BorderTop
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public short BottomBorderColor
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void CloneStyleFrom(ICellStyle source)
        {
            throw new NotImplementedException();
        }

        public short DataFormat
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public short FillBackgroundColor
        {
            get;
            set;
        }

        public short FillForegroundColor
        {
            get;
            set;
        }

        public FillPatternType FillPattern
        {
            get;
            set;
        }

        public short FontIndex
        {
            get { throw new NotImplementedException(); }
        }

        public string GetDataFormatString()
        {
            throw new NotImplementedException();
        }

        public IFont GetFont(IWorkbook parentWorkbook)
        {
            throw new NotImplementedException();
        }

        public short Indention
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public short Index
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsHidden
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool IsLocked
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public short LeftBorderColor
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public short RightBorderColor
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public short Rotation
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void SetFont(IFont font)
        {
            throw new NotImplementedException();
        }

        public bool ShrinkToFit
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public short TopBorderColor
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public VerticalAlignment VerticalAlignment
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool WrapText
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public NPOI.SS.UserModel.FontBoldWeight FontWeight
        {
            get;
            set;
        }
    }
}
