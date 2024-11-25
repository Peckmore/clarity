using System;
using System.Drawing;
using System.Windows.Forms;

namespace Clarity
{
    public class ClarityTextRenderEventArgs : ClarityRenderEventArgs
    {
        #region Fields

        private Boolean _isTitle;
        private String _text;
        private Font _textFont;
        private Rectangle _textRectangle;
        
        #endregion

        #region Properties

        public Boolean IsTitle
        {
            get { return _isTitle; }
        }
        public String Text
        {
            get { return _text; }
        }
        public Font TextFont
        {
            get { return _textFont; }
        }
        public Rectangle TextRectangle
        {
            get { return _textRectangle; }
        }

        #endregion

        #region Constructors

        public ClarityTextRenderEventArgs(Graphics g, Form clarityForm, Form primaryClarityForm, String text, Font textFont, Rectangle textRectangle, Boolean isTitle)
            : base(g, clarityForm, primaryClarityForm)
        {
            _text = text;
            _textFont = textFont;
            _textRectangle = textRectangle;
            _isTitle = isTitle;
        }

        #endregion
    }
}