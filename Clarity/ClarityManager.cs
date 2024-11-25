using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Clarity
{
    public static class ClarityManager
    {
        #region Fields

        private static Boolean _clientAreaOnly;
        private static Font _descriptionFont;
        private static Boolean _hotTrack;
        private static ClarityForm _primaryForm;
        private static ClarityRenderer _renderer;
        private static ClarityRenderMode _renderMode;
        private static List<String> _textFormatExclusions = new List<String>();
        private static Font _titleFont;
        private static Boolean _visible;

        #endregion

        #region Properties

        public static Boolean ClientAreaOnly
        {
            get { return _clientAreaOnly; }
            set { _clientAreaOnly = value; }
        }
        public static Font DescriptionFont
        {
            get { return _descriptionFont; }
            set { _descriptionFont = value; }
        }
        public static Boolean HotTrack
        {
            get { return _hotTrack; }
            set { _hotTrack = value; }
        }
        public static ClarityRenderer Renderer
        {
            get { return _renderer; }
            set
            {
                _renderer = value;
                _renderMode = ClarityRenderMode.Custom;
            }
        }
        public static ClarityRenderMode RenderMode
        {
            get { return _renderMode; }
            set
            {
                _renderMode = value;
                switch (_renderMode)
                {
                    case ClarityRenderMode.Light:
                        _renderer = new ClarityLightRenderer();
                        break;
                    case ClarityRenderMode.Dark:
                        _renderer = new ClarityDarkRenderer();
                        break;
                }
            }
        }
        public static List<String> TextFormatExclusions
        {
            get { return _textFormatExclusions; }
        }
        public static Font TitleFont
        {
            get { return _titleFont; }
            set { _titleFont = value; }
        }
        public static Boolean Visible
        {
            get { return _visible; }
        }

        #endregion

        #region Events

        public static event ClarityClosedEventHandler Closed;

        #endregion

        static ClarityManager()
        {
            RenderMode = ClarityRenderMode.Light;
            DescriptionFont = new Font("Segoe UI", 16, FontStyle.Bold);
            TitleFont = new Font("Segoe UI", 40, FontStyle.Regular);
        }

        #region Methods

        #region Event Handlers

        static void _primaryForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _primaryForm.FormClosed -= _primaryForm_FormClosed;

            if (Closed != null)
                Closed(null, new EventArgs());
        }

        #endregion

        #region Private Static

        private static void Show(Boolean welcome, Int32 state, String initialTitle, String initialDescription, IClarityProvider clarityProvider)
        {
            _primaryForm = new ClarityForm(welcome, state, initialTitle, initialDescription, clarityProvider, null);
            _primaryForm.Show();
            _primaryForm.FormClosed += _primaryForm_FormClosed;
            _visible = true;
        }

        #endregion

        #region Public

        public static void Show(Int32 state, String initialTitle, String initialDescription, IClarityProvider clarityProvider)
        {
            Show(false, state, initialTitle, initialDescription, clarityProvider);
        }
        public static void ShowWelcome(IClarityProvider clarityProvider)
        {
            Show(true, 0, String.Empty, String.Empty, clarityProvider);
        }

        #endregion

        #endregion
    }
}