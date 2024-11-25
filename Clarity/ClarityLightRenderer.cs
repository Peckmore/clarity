using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Clarity
{
    public class ClarityLightRenderer : ClarityRenderer
    {
        #region Constants


        #endregion

        #region Fields

        private Color _backColor;
        private Color _foreColor;
        private ImageAttributes _imageAttributes;
        private StringFormat _textFormat;

        #endregion

        #region Properties

        public virtual Color BackColor
        {
            get { return _backColor; }
        }
        public virtual Color ForeColor
        {
            get { return _foreColor; }
        }

        #endregion

        #region Constructors

        public ClarityLightRenderer()
            : this(Color.WhiteSmoke, Color.Black)
        { }
        public ClarityLightRenderer(Color backColor, Color foreColor)
        {
            _backColor = backColor;
            _foreColor = foreColor;

            // Set text alignment for when we draw text later
            _textFormat = new StringFormat();
            _textFormat.Alignment = StringAlignment.Center;

            ColorMap[] colorMap = new ColorMap[1];
            colorMap[0] = new ColorMap();
            colorMap[0].OldColor = Color.Black;
            colorMap[0].NewColor = ForeColor;
            _imageAttributes = new ImageAttributes();
            _imageAttributes.SetRemapTable(colorMap);
        }

        #endregion

        #region Methods

        #region Protected

        protected override void OnDispose()
        {
            _imageAttributes.Dispose();
            _textFormat.Dispose();
        }
        protected override String OnFormatText(String text)
        {
            String response = text;

            if (!String.IsNullOrEmpty(response))
            {
                response = response.ToUpperInvariant();
                response = response.Replace(".", "\n\n");
                response = response.Replace("clarity", "Clarity");

                if (ClarityManager.TextFormatExclusions != null)
                    foreach (String s in ClarityManager.TextFormatExclusions)
                        response = response.Replace(s.ToUpperInvariant(), s);
            }

            return response;
        }
        protected override void OnRenderBackground(ClarityRenderEventArgs e)
        {
            e.Graphics.Clear(BackColor);
        }
        protected override void OnRenderText(ClarityTextRenderEventArgs e)
        {
            if (e.IsPrimaryForm)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, ForeColor)), e.TextRectangle);

                SetGraphicsOptions(e.Graphics, InterpolationMode.HighQualityBilinear, SmoothingMode.HighQuality);

                Int32 _textBlurWidth = -1;
                for (int x = -_textBlurWidth; x <= _textBlurWidth; x++)
                    for (int y = -_textBlurWidth; y <= _textBlurWidth; y++)
                        e.Graphics.DrawString(e.Text, e.TextFont, new SolidBrush(Color.FromArgb(15, Color.Black)), new Rectangle(new Point(e.TextRectangle.X + x, e.TextRectangle.Y + y), e.TextRectangle.Size), _textFormat);

                e.Graphics.DrawString(e.Text, e.TextFont, new SolidBrush(Color.White), e.TextRectangle, _textFormat);
                RestoreGraphicsOptions(e.Graphics);
            }
        }
        protected override void OnRenderClarityItem(ClarityItemRenderEventArgs e)
        {
            if (e.ClarityItem.Image != null)
                e.Graphics.DrawImage(e.ClarityItem.Image, e.ClarityItem.VisibleBounds);

            if (!e.IsActiveItem)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(96, e.ClarityItem.AverageColor)), e.ClarityItem.VisibleBounds);
                //e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(32, BackColor)), e.ClarityItem.VisibleBounds);
                if (!e.ClarityItem.IsWindow)
                    //e.Graphics.DrawRectangle(new Pen(ControlPaint.Light(e.ClarityItem.AverageColor), 1), new Rectangle(e.ClarityItem.VisibleBounds.Location.X, e.ClarityItem.VisibleBounds.Y, e.ClarityItem.VisibleBounds.Width - 1, e.ClarityItem.VisibleBounds.Height - 1));
                    e.Graphics.DrawRectangle(new Pen(Color.FromArgb(255, 241, 143, 49), 1), new Rectangle(e.ClarityItem.VisibleBounds.Location.X, e.ClarityItem.VisibleBounds.Y, e.ClarityItem.VisibleBounds.Width - 1, e.ClarityItem.VisibleBounds.Height - 1));
            }
            else if (!e.ClarityItem.IsWindow)
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(128, ControlPaint.Dark(e.ClarityItem.AverageColor)), 1), new Rectangle(e.ClarityItem.VisibleBounds.Location.X, e.ClarityItem.VisibleBounds.Y, e.ClarityItem.VisibleBounds.Width - 1, e.ClarityItem.VisibleBounds.Height - 1));
        }

        #endregion

        #endregion
    }
}