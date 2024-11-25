using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Clarity
{
    internal partial class ClarityForm : Form, IDisposable
    {
        #region Constants

        private const Single _maxOpacity = 0.80f;
        private const Int32 _textOffset = 60;

        #endregion

        #region Fields

        private ClarityItem _activeClarityItem;
        private List<ClarityForm> _childClarityForms;
        private List<ClarityItem> _clarityItems;
        private IntPtr _controlClickHandle;
        private Point _controlClickLocation;
        private Boolean _coversControls;
        private Form _clarityProviderForm;
        private ClarityItem _defaultItem;
        private Boolean _isFadingOut;
        private Boolean _isShown;
        private ClarityForm _primaryClarityForm;
        private Boolean _welcome;


        private Rectangle _clarityTextBounds;
        private Rectangle _titleBounds;

        #endregion

        #region Properties

        public ClarityItem ActiveClarityItem
        {
            get { return _activeClarityItem; }
            set
            {
                if (PrimaryForm.ActiveClarityItem != value)
                {
                    if (IsPrimaryForm)
                        _activeClarityItem = value;
                    else
                        PrimaryForm.ActiveClarityItem = value;
                    Refresh();
                }
            }
        }
        public Boolean IsPrimaryForm
        {
            get { return PrimaryForm == this; }
        }
        public Boolean IsShown
        {
            get { return _isShown; }
        }
        public ClarityForm PrimaryForm
        {
            get { return _primaryClarityForm; }
            set
            {
                if (value == null)
                    _primaryClarityForm = this;
                else
                    _primaryClarityForm = value;
            }
        }

        #endregion

        #region Constructors

        internal ClarityForm(Boolean welcome, Int32 state, String initialTitle, String initialDescription, IClarityProvider clarityProvider, ClarityForm parent)
        {
            _clarityProviderForm = clarityProvider as Form;
            if (_clarityProviderForm == null)
                throw new ArgumentException("clarityProvider must be a form.");
            if (_clarityProviderForm.IsDisposed)
                throw new ObjectDisposedException(_clarityProviderForm.Name, "clarityProvider has been disposed.");

            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            _welcome = welcome;

            _clarityProviderForm.FormClosed += _clarityProviderForm_FormClosed;
            _clarityProviderForm.Move += _clarityProviderForm_Move;
            _clarityProviderForm.SizeChanged += _clarityProviderForm_SizeChanged;

            

            // Set the 'primary' Clarity form (used if there are child forms)
            PrimaryForm = parent;

            // Create child forms for each child Clarity Provider given
            if (!_welcome)
            {
                _childClarityForms = new List<ClarityForm>();
                if (clarityProvider.GetChildClarityProviders(state) != null)
                    foreach (IClarityProvider d in clarityProvider.GetChildClarityProviders(state))
                        try
                        {
                            _childClarityForms.Add(new ClarityForm(_welcome, state, initialTitle, initialDescription, d, this));
                        }
                        catch (ObjectDisposedException)
                        {
                            // Catch an error upon construction whereby the child IClarityProvider has been disposed.
                            // No need to do anything as the form isn't created and so isn't added to the list.
                        }

                // Get the Clarity items available from the Clarity Provider
                _clarityItems = clarityProvider.GetClarityItems(state);
                _defaultItem = new ClarityItem(initialTitle, initialDescription);
                PrimaryForm.ActiveClarityItem = _defaultItem;
            }

            if (_clarityItems == null)
                _clarityItems = new List<ClarityItem>();

            //_maximised = (_clarityProviderForm.TopLevelControl as Form).WindowState == FormWindowState.Maximized;

            // Set the Size and Location of our form based on the specified on-screen element
            if (ClarityManager.ClientAreaOnly)
                this.Size = _clarityProviderForm.TopLevelControl.ClientRectangle.Size;
            else
                this.Size = _clarityProviderForm.TopLevelControl.Size;

            this.Location = _clarityProviderForm.TopLevelControl.Location;
            Point tempLocation = _clarityProviderForm.PointToScreen(new Point(0, 0));
            Point offset = new Point(tempLocation.X - _clarityProviderForm.TopLevelControl.Left - (false ? 8 : 0), tempLocation.Y - _clarityProviderForm.TopLevelControl.Top - (false ? 8 : 0));

            if (ClarityManager.ClientAreaOnly)
                this.Location = new Point(Location.X + offset.X, Location.Y + offset.Y);
            else
                foreach (ClarityItem i in _clarityItems)
                    if (!i.IsWindow)
                        i.VisibleBounds = new Rectangle(i.VisibleBounds.Location.X + offset.X, i.VisibleBounds.Location.Y + offset.Y, i.VisibleBounds.Width, i.VisibleBounds.Height);

            // Needs to be done after the size of the form has been set
            //_welcomeImageBounds = new Rectangle((Width - Properties.Resources.ClarityLarge.Width) / 2, (Height - Properties.Resources.ClarityLarge.Height) / 2, Properties.Resources.ClarityLarge.Width, Properties.Resources.ClarityLarge.Height);
        }

        #endregion

        #region Methods

        #region Event Handlers

        private void _clarityProviderForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            PrimaryForm.Close();
        }
        private void _clarityProviderForm_Move(object sender, EventArgs e)
        {
            PrimaryForm.Close();
        }
        private void _clarityProviderForm_SizeChanged(object sender, EventArgs e)
        {
            PrimaryForm.Close();
        }
        private void ClarityForm_Activated(object sender, EventArgs e)
        {
            if (ClarityManager.HotTrack)
                PrimaryForm.StopDeactivationTimer();
        }
        private void ClarityForm_Deactivate(object sender, EventArgs e)
        {
            if (PrimaryForm.IsShown)
                PrimaryForm.StartDeactivationTimer();
        }
        private void ClarityForm_MouseClick(object sender, MouseEventArgs e)
        {
            //if (!_welcome)
                if (PrimaryForm.ActiveClarityItem != _defaultItem)
                {
                    if (PrimaryForm.ActiveClarityItem.HelpObject != null)
                    {
                        _controlClickHandle = PrimaryForm.ActiveClarityItem.ControlHandle;
                        _controlClickLocation = Control.FromHandle(PrimaryForm.ActiveClarityItem.ControlHandle).PointToClient(this.PointToScreen(e.Location));
                        PrimaryForm.Close();
                    }
                }
                else
                    PrimaryForm.Close();
        }
        private void ClarityForm_MouseEnter(object sender, EventArgs e)
        {
            if (ClarityManager.HotTrack)
                this.Activate();
        }
        private void ClarityForm_MouseLeave(object sender, EventArgs e)
        {
            if (!_welcome)
            {
                ActiveClarityItem = _defaultItem;
                Cursor = Cursors.Default;
            }
        }
        private void ClarityForm_MouseMove(object sender, MouseEventArgs e)
        {
            Boolean objectFound = false;

            foreach (ClarityItem helpObject in _clarityItems)
                if (helpObject.VisibleBounds.Contains(e.Location))
                {
                    ActiveClarityItem = helpObject;
                    this.Cursor = Cursors.Hand;
                    objectFound = true;
                    break;
                }

            if (!objectFound)
            {
                ActiveClarityItem = _defaultItem;
                this.Cursor = Cursors.Default;
            }

            if (!_welcome && IsPrimaryForm && _coversControls)
            {
                if ((_titleBounds.Contains(e.Location) || _clarityTextBounds.Contains(e.Location)))
                {
                    if (_titleBounds.Y == _textOffset)
                    {
                        _titleBounds.Y = (Height / 2) + _textOffset;
                        _clarityTextBounds.Y = (Height / 2) + _textOffset + _titleBounds.Height;
                    }
                    else if (_titleBounds.Y > _textOffset)
                    {
                        _titleBounds.Y = _textOffset;
                        _clarityTextBounds.Y = +_textOffset + _titleBounds.Height;
                    }
                    Refresh();
                }
            }
        }
        private void ClarityForm_Shown(object sender, EventArgs e)
        {
            timerFade.Start();

            if (_childClarityForms != null)
                foreach (ClarityForm cf in _childClarityForms)
                    cf.Show();

            if (IsPrimaryForm)
                _isShown = true;
        }
        private void ClarityForm_SizeChanged(object sender, EventArgs e)
        {
            _titleBounds = new Rectangle(0, (Height / 2) + _textOffset, Width, 76);
            _clarityTextBounds = new Rectangle(0, (Height / 2) + _textOffset + _titleBounds.Height, Width, (Height / 3) - _titleBounds.Height);
            if (_clarityItems?.Any() == true)
            {
                foreach (ClarityItem i in _clarityItems)
                    if (_titleBounds.IntersectsWith(i.VisibleBounds) || _clarityTextBounds.IntersectsWith(i.VisibleBounds))
                    {
                        _coversControls = true;
                        break;
                    }
            }

            ClarityManager.Renderer.SizeChanged(new ClaritySizeChangedEventArgs(_titleBounds, _clarityTextBounds));
        }
        private void timerDeactivate_Tick(object sender, EventArgs e)
        {
            timerDeactivate.Stop();
            PrimaryForm.Close();
        }
        private void timerFade_Tick(object sender, EventArgs e)
        {
            if (_isFadingOut)
            {
                if (this.Opacity <= 0)
                {
                    timerFade.Stop();

                    // HACK: Find out why this is called twice
                    //_isFadingOut = false;

                    Debug.WriteLine(DateTime.Now.ToString() + " - Fading out");
                    base.Close();

                    Win32.SetForegroundWindow(_controlClickHandle);
                    Win32.SendMessage(_controlClickHandle, (Int32)Win32.WM_LBUTTONDOWN, 0x1, ((_controlClickLocation.Y << 16) | (_controlClickLocation.X & 0xFFFF)));
                    Win32.SendMessage(_controlClickHandle, (Int32)Win32.WM_LBUTTONUP, 0x1, ((_controlClickLocation.Y << 16) | (_controlClickLocation.X & 0xFFFF)));

                    this.Dispose();
                }
                this.Opacity -= 0.1;
            }
            else
            {
                Debug.WriteLine(DateTime.Now.ToString() + " - Fading in");

                this.Opacity += 0.1;
                if (this.Opacity >= _maxOpacity)
                {
                    this.Opacity = _maxOpacity;
                    _isFadingOut = true;
                    timerFade.Stop();
                }
            }
        }

        #endregion

        #region Private

        private void SetClientArea(ref Win32.RECT clientArea)
        {
            clientArea.Left += 8;
            clientArea.Top += 8;
            clientArea.Right -= 8;
            clientArea.Bottom -= 16;
        }

        #endregion

        #region Protected

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;
                createParams.Style = unchecked((Int32)Win32.WS_POPUP) | Win32.WS_CLIPSIBLINGS | Win32.WS_CLIPCHILDREN;
                createParams.ExStyle = Win32.WS_EX_LAYERED | Win32.WS_EX_TOOLWINDOW;
                return createParams;
            }
        }
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();

                foreach (ClarityItem item in _clarityItems)
                    item.Dispose();

                if (_defaultItem != null)
                    _defaultItem.Dispose();

                timerDeactivate.Dispose();
                timerFade.Dispose();
            }
            base.Dispose(disposing);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

            if (_welcome && IsPrimaryForm)
            {
                ClarityManager.Renderer.DrawText(new ClarityTextRenderEventArgs(e.Graphics, this, PrimaryForm, ClarityManager.Renderer.FormatText("introducing Clarity"), ClarityManager.TitleFont, _titleBounds, true));
                ClarityManager.Renderer.DrawText(new ClarityTextRenderEventArgs(e.Graphics, this, PrimaryForm, ClarityManager.Renderer.FormatText("\nthe icon below will always be displayed on this window\n\nyou can press this icon any time you want Clarity"), ClarityManager.DescriptionFont, _clarityTextBounds, false));
                Rectangle secondaryText = new Rectangle(_clarityTextBounds.Location, _clarityTextBounds.Size);
                secondaryText.Offset(0, (Int32)(Height * 0.65));
                ClarityManager.Renderer.DrawText(new ClarityTextRenderEventArgs(e.Graphics, this, PrimaryForm, ClarityManager.Renderer.FormatText("Clarity will highlight on-screen items that are relevant to the activity you are carrying out\n\nhover over any of the highlighted items to view a brief description\n\nclick on the item to open further help"), ClarityManager.DescriptionFont, secondaryText, false));
                //e.Graphics.DrawImage(Properties.Resources.ClarityLarge, new Point((Width - Properties.Resources.ClarityLarge.Width) / 2, (Height - Properties.Resources.ClarityLarge.Height) / 2));
                //e.Graphics.DrawImage(Properties.Resources.ClarityLarge, _welcomeImageBounds, 0, 0, Properties.Resources.ClarityLarge.Width, Properties.Resources.ClarityLarge.Height, GraphicsUnit.Pixel, _imageAttributes);
            }
            else
            {
                foreach (ClarityItem helpObject in _clarityItems)
                    if (helpObject != PrimaryForm.ActiveClarityItem)
                        ClarityManager.Renderer.DrawClarityItem(new ClarityItemRenderEventArgs(e.Graphics, this, PrimaryForm, helpObject, false));

                if (_clarityItems.Contains(PrimaryForm.ActiveClarityItem))
                    ClarityManager.Renderer.DrawClarityItem(new ClarityItemRenderEventArgs(e.Graphics, this, PrimaryForm, PrimaryForm.ActiveClarityItem, true));

                ClarityManager.Renderer.DrawText(new ClarityTextRenderEventArgs(e.Graphics, this, PrimaryForm, ClarityManager.Renderer.FormatText(PrimaryForm.ActiveClarityItem.Title), ClarityManager.TitleFont, _titleBounds, true));
                ClarityManager.Renderer.DrawText(new ClarityTextRenderEventArgs(e.Graphics, this, PrimaryForm, ClarityManager.Renderer.FormatText(PrimaryForm.ActiveClarityItem.Description), ClarityManager.DescriptionFont, _clarityTextBounds, false));
            }
        }        
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            ClarityManager.Renderer.DrawBackground(new ClarityRenderEventArgs(e.Graphics, this, PrimaryForm));
        }
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            //switch (m.Msg)
            //{
            //    //case (Int32)Win32.WM_WINDOWPOSCHANGING:
            //    //    Win32.WINDOWPOS windowPos = (Win32.WINDOWPOS)Marshal.PtrToStructure(m.LParam, typeof(Win32.WINDOWPOS));
            //    //    Rectangle screenBounds = Screen.GetBounds(this);
            //    //    maximizeDetected = screenBounds.X - windowPos.x == 8 && screenBounds.Y - windowPos.y == 8 && screenBounds.Width - windowPos.cx == -16 && screenBounds.Height - windowPos.cy == -16;
            //    //    break;
            //    case (Int32)Win32.WM_NCCALCSIZE:
            //        if (_maximised)
            //        {
            //            if (m.WParam == IntPtr.Zero)
            //            {
            //                Win32.RECT rcsize = (Win32.RECT)Marshal.PtrToStructure(m.LParam, typeof(Win32.RECT));
            //                SetClientArea(ref rcsize);
            //                Marshal.StructureToPtr(rcsize, m.LParam, false);
            //            }
            //            else
            //            {
            //                Win32.NCCALCSIZE_PARAMS rcsize = (Win32.NCCALCSIZE_PARAMS)Marshal.PtrToStructure(m.LParam, typeof(Win32.NCCALCSIZE_PARAMS));
            //                SetClientArea(ref rcsize.rcNewWindow);
            //                Marshal.StructureToPtr(rcsize, m.LParam, false);
            //            }
            //            return;
            //        }
            //        break;
            //}
        }
        #endregion

        #region Public

        public new void Close()
        {
            _clarityProviderForm.FormClosed -= _clarityProviderForm_FormClosed;
            _clarityProviderForm.Move -= _clarityProviderForm_Move;
            _clarityProviderForm.SizeChanged -= _clarityProviderForm_SizeChanged;

            if (_childClarityForms != null)
                foreach (ClarityForm cf in _childClarityForms)
                    cf.Close();

            timerFade.Interval = 3;
            timerFade.Start();
        }
        public void StartDeactivationTimer()
        {
            timerDeactivate.Start();
        }
        public void StopDeactivationTimer()
        {
            timerDeactivate.Stop();
        }

        #endregion

        #endregion
    }
}