using System;
using System.Drawing;
using System.Windows.Forms;

namespace Clarity
{
    public class ClarityRenderEventArgs : EventArgs
    {
        #region Fields

        private Form _clarityForm;
        private Graphics _graphics;
        private Boolean _isPrimaryForm;
        private Form _primaryClarityForm;

        #endregion

        #region Properties

        public Form ClarityForm
        {
            get { return _clarityForm; }
        }
        public Graphics Graphics
        {
            get { return _graphics; }
        }
        public Boolean IsPrimaryForm
        {
            get { return _isPrimaryForm; }
        }
        public Form PrimaryClarityForm
        {
            get { return _primaryClarityForm; }
        }

        #endregion

        #region Constructors

        public ClarityRenderEventArgs(Graphics g, Form clarityForm, Form primaryClarityForm)
        {
            _graphics = g;
            _isPrimaryForm = clarityForm == primaryClarityForm;
            _primaryClarityForm = primaryClarityForm;
        }

        #endregion
    }
}