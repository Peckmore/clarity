using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Clarity
{
    public abstract class ClarityRenderer : IDisposable
    {
        #region Fields

        private InterpolationMode _graphicsInterpolationMode;
        private SmoothingMode _graphicsSmoothingMode;
        private TextRenderingHint _graphicsTextRenderingHint;

        #endregion

        #region Events

        public event EventHandler RenderBackground;
        public event EventHandler RenderClarityItem;
        public event EventHandler RenderText;

        #endregion

        #region Methods

        #region Protected

        protected virtual void OnDispose()
        { }
        protected virtual String OnFormatText(String text)
        {
            return text;
        }
        protected virtual void OnRenderBackground(ClarityRenderEventArgs e)
        { }
        protected virtual void OnRenderClarityItem(ClarityItemRenderEventArgs e)
        { }
        protected virtual void OnRenderText(ClarityTextRenderEventArgs e)
        { }
        protected virtual void OnSizeChanged(ClaritySizeChangedEventArgs e)
        { }
        protected void RestoreGraphicsOptions(Graphics g)
        {
            g.InterpolationMode = _graphicsInterpolationMode;
            g.SmoothingMode = _graphicsSmoothingMode;
            g.TextRenderingHint = _graphicsTextRenderingHint;
        }
        protected void SetGraphicsOptions(Graphics g, InterpolationMode interpolationMode)
        {
            SetGraphicsOptions(g, interpolationMode, g.SmoothingMode, g.TextRenderingHint);
        }
        protected void SetGraphicsOptions(Graphics g, SmoothingMode smoothingMode)
        {
            SetGraphicsOptions(g, g.InterpolationMode, smoothingMode, g.TextRenderingHint);
        }
        protected void SetGraphicsOptions(Graphics g, TextRenderingHint textRenderingHint)
        {
            SetGraphicsOptions(g, g.InterpolationMode, g.SmoothingMode, textRenderingHint);
        }
        protected void SetGraphicsOptions(Graphics g, InterpolationMode interpolationMode, SmoothingMode smoothingMode)
        {
            SetGraphicsOptions(g, interpolationMode, smoothingMode, g.TextRenderingHint);
        }
        protected void SetGraphicsOptions(Graphics g, InterpolationMode interpolationMode, TextRenderingHint textRenderingHint)
        {
            SetGraphicsOptions(g, interpolationMode, g.SmoothingMode, textRenderingHint);
        }
        protected void SetGraphicsOptions(Graphics g, SmoothingMode smoothingMode, TextRenderingHint textRenderingHint)
        {
            SetGraphicsOptions(g, g.InterpolationMode, smoothingMode, textRenderingHint);
        }
        protected void SetGraphicsOptions(Graphics g, InterpolationMode interpolationMode, SmoothingMode smoothingMode, TextRenderingHint textRenderingHint)
        {
            _graphicsInterpolationMode = g.InterpolationMode;
            _graphicsSmoothingMode = g.SmoothingMode;
            _graphicsTextRenderingHint = g.TextRenderingHint;

            g.InterpolationMode = interpolationMode;
            g.SmoothingMode = smoothingMode;
            g.TextRenderingHint = textRenderingHint;
        }
        
        #endregion

        #region Public

        public void Dispose()
        {
            OnDispose();
        }
        public void DrawBackground(ClarityRenderEventArgs e)
        {
            OnRenderBackground(e);
            EventHandler handler = RenderBackground;
            if (handler != null)
                handler(this, e);
        }
        public void DrawClarityItem(ClarityItemRenderEventArgs e)
        {
            OnRenderClarityItem(e);
            EventHandler handler = RenderClarityItem;
            if (handler != null)
                handler(this, e);
        }
        public void DrawText(ClarityTextRenderEventArgs e)
        {
            OnRenderText(e);
            EventHandler handler = RenderText;
            if (handler != null)
                handler(this, e);
        }
        public String FormatText(String text)
        {
            return OnFormatText(text);
        }
        public void SizeChanged(ClaritySizeChangedEventArgs e)
        {
            OnSizeChanged(e);
        }

        #endregion

        #endregion
    }
}