using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Clarity
{
    public class ClarityItem : IDisposable
    {
        #region Fields

        private Color _averageColor;
        private IntPtr _controlHandle;
        private String _description;
        private Uri _helpObject;
        private Bitmap _image;
        private Boolean _isWindow;
        private String _title;
        private Rectangle _visibleBounds;

        #endregion

        #region Properties

        public Color AverageColor
        {
            get { return _averageColor; }
        }
        public IntPtr ControlHandle
        {
            get { return _controlHandle; }
        }
        public String Description
        {
            get { return _description; }
        }
        public Uri HelpObject
        {
            get { return _helpObject; }
        }
        public Image Image
        {
            get { return _image; }
        }
        public Boolean IsWindow
        {
            get { return _isWindow; }
        }
        public String Title
        {
            get { return _title; }
        }
        public Rectangle VisibleBounds
        {
            get { return _visibleBounds; }
            internal set { _visibleBounds = value; }
        }

        #endregion

        #region Constructors

        #region Internal

        internal ClarityItem(String title, String description)
        {
            CreateClarityItem(title, description, null, null, Rectangle.Empty, Rectangle.Empty);
        }

        #endregion

        #region Public

        public ClarityItem(String title, String description, Uri helpObject, Control control)
        {
            Rectangle controlBounds = Rectangle.Empty;
            Rectangle visibleBounds = Rectangle.Empty;

            if (control != null)
            {
                control.TopLevelControl.BringToFront();

                if (control.Parent == null)
                {
                    // Control bounds relative to root parent (Form)
                    controlBounds = new Rectangle(new Point(0, 0), control.Bounds.Size);

                    // Visible control bounds relative to root parent (Form)
                    visibleBounds = controlBounds;
                }
                else
                {
                    // Control bounds relative to root parent (Form)
                    controlBounds = new Rectangle(control.TopLevelControl.PointToClient(control.Parent.PointToScreen(control.Location)), control.Size);

                    // Visible control bounds relative to root parent (Form)
                    visibleBounds = control.Bounds;
                    visibleBounds.Intersect(control.Parent.ClientRectangle);
                    visibleBounds = new Rectangle(control.TopLevelControl.PointToClient(control.Parent.PointToScreen(visibleBounds.Location)), visibleBounds.Size);
                }
            }

            CreateClarityItem(title, description, helpObject, control, controlBounds, visibleBounds);
        }
        public ClarityItem(String title, String description, Uri helpObject, ToolStripItem toolStripItem)
        {
            CreateClarityItemFromElement(title, description, helpObject, toolStripItem.Bounds, toolStripItem.Owner, toolStripItem.Visible);
        }
        public ClarityItem(String title, String description, Uri helpObject, TreeNode treeNode)
        {
            CreateClarityItemFromElement(title, description, helpObject, treeNode.Bounds, treeNode.TreeView, treeNode.IsVisible);
        }

        #endregion

        #endregion

        #region Methods

        #region Private

        private void CalculateAverageColor()
        {
            Int32 droppedPixels = 0;
            Int64[] colorTotals = new Int64[] { 0, 0, 0 };
            BitmapData bitmapData = _image.LockBits(new Rectangle(0, 0, _image.Width, _image.Height), ImageLockMode.ReadOnly, _image.PixelFormat);

            try
            {
                unsafe
                {
                    Int32 currentPixelR = 0;
                    Int32 currentPixelG = 0;
                    Int32 currentPixelB = 0;
                    Int32 differenceThreshold = 15; // don't count pixels that do not differ by at least this value

                    Byte* p = (Byte*)(void*)bitmapData.Scan0;

                    for (Int32 y = 0; y < _image.Height; y++)
                    {
                        for (Int32 x = 0; x < _image.Width; x++)
                        {
                            Int32 currentPixel = (y * bitmapData.Stride) + x * 4; // 4 is used as 32-bit images are assumed
                            currentPixelR = p[currentPixel + 2];
                            currentPixelG = p[currentPixel + 1];
                            currentPixelB = p[currentPixel];

                            if (Math.Abs(currentPixelR - currentPixelG) > differenceThreshold || Math.Abs(currentPixelR - currentPixelB) > differenceThreshold || Math.Abs(currentPixelG - currentPixelB) > differenceThreshold)
                            {
                                colorTotals[2] += currentPixelR;
                                colorTotals[1] += currentPixelG;
                                colorTotals[0] += currentPixelB;
                            }
                            else
                                droppedPixels++;
                        }
                    }
                }
            }
            finally
            {
                _image.UnlockBits(bitmapData);
            }

            Int32 pixelCount = _image.Width * _image.Height - droppedPixels;
            if (pixelCount == 0)
                pixelCount += 1;

            Int32 averageR = (Int32)(colorTotals[2] / pixelCount);
            Int32 averageG = (Int32)(colorTotals[1] / pixelCount);
            Int32 averageB = (Int32)(colorTotals[0] / pixelCount);

            _averageColor = Color.FromArgb(averageR, averageG, averageB);
        }
        private void CreateClarityItem(String title, String description, Uri helpObject, Control control, Rectangle controlBounds, Rectangle visibleBounds)
        {
            _title = title;
            _description = description;
            _helpObject = helpObject;

            if (control != null)
            {
                _controlHandle = control.Handle;
                _isWindow = control.Parent == null;

                _visibleBounds = visibleBounds;

                if (_visibleBounds.Width > 0 && _visibleBounds.Height > 0)
                {
                    _image = new Bitmap(_visibleBounds.Width, _visibleBounds.Height);
                    using (Graphics bitmapGraphics = Graphics.FromImage(_image))
                    {
                        if (control.Parent == null && !ClarityManager.ClientAreaOnly)
                        {
                            Form form = control as Form;
                            Boolean currentTopMost = form.TopMost;
                            form.TopMost = true;
                            control.BringToFront();
                            bitmapGraphics.CopyFromScreen(control.Location, Point.Empty, _visibleBounds.Size);
                            form.TopMost = currentTopMost;
                        }
                        else
                        {
                            IntPtr hDC = IntPtr.Zero;
                            try
                            {
                                hDC = control.CreateGraphics().GetHdc(); //hDC = Win32.GetWindowDC(control.Handle); // For entire form
                                IntPtr bitmapHandle = bitmapGraphics.GetHdc();
                                Win32.BitBlt(bitmapHandle, 0, 0, _visibleBounds.Width, _visibleBounds.Height, hDC, _visibleBounds.X - controlBounds.X, _visibleBounds.Y - controlBounds.Y, 13369376);
                                bitmapGraphics.ReleaseHdc(bitmapHandle);
                            }
                            finally
                            {
                                if (hDC != IntPtr.Zero)
                                    Win32.ReleaseDC(_controlHandle, hDC);
                            }
                        }
                    }

                    if (_image != null)
                        CalculateAverageColor();
                }
            }
        }
        private void CreateClarityItemFromElement(String title, String description, Uri helpObject, Rectangle elementBounds, Control owner, Boolean isVisible)
        {
            Rectangle controlBounds = Rectangle.Empty;
            Rectangle visibleBounds = Rectangle.Empty;

            if (isVisible)
            {
                owner.TopLevelControl.BringToFront();

                Rectangle toolStripItemBounds = new Rectangle(new Point(owner.Location.X + elementBounds.X, owner.Location.Y + elementBounds.Y), elementBounds.Size);

                // Control bounds relative to root parent (Form)
                controlBounds = new Rectangle(owner.TopLevelControl.PointToClient(owner.Parent.PointToScreen(owner.Location)), toolStripItemBounds.Size);

                // Visible control bounds relative to root parent (Form)
                visibleBounds = toolStripItemBounds;
                visibleBounds.Intersect(owner.Parent.ClientRectangle);
                visibleBounds = new Rectangle(owner.TopLevelControl.PointToClient(owner.Parent.PointToScreen(visibleBounds.Location)), visibleBounds.Size);

                CreateClarityItem(title, description, helpObject, owner, controlBounds, visibleBounds);
            }
            else
                CreateClarityItem(title, description, helpObject, owner, Rectangle.Empty, Rectangle.Empty);
        }

        #endregion

        #region Public

        public void Dispose()
        {
            if (_image != null)
                _image.Dispose();
        }

        #endregion

        #endregion
    }
}