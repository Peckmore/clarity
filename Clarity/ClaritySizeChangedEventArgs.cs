using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clarity
{
    public class ClaritySizeChangedEventArgs : EventArgs
    {
        #region Fields

        private Rectangle _descriptionRectangle;
        private Rectangle _titleRectangle;

        #endregion

        #region Properties

        public Rectangle DescriptionRectangle
        {
            get { return _descriptionRectangle; }
        }
        public Rectangle TitleRectangle
        {
            get { return _titleRectangle; }
        }

        #endregion

        #region Consutrctors

        public ClaritySizeChangedEventArgs(Rectangle titleRectangle, Rectangle descriptionRectangle)
        {
            _titleRectangle = titleRectangle;
            _descriptionRectangle = descriptionRectangle;
        }

        #endregion
    }
}
