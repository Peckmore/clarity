using System;
using System.Drawing;
using System.Windows.Forms;

namespace Clarity
{
    public class ClarityItemRenderEventArgs : ClarityRenderEventArgs
    {
        #region Fields

        private ClarityItem _clarityItem;
        private Boolean _isActiveItem;

        #endregion

        #region Properties

        public ClarityItem ClarityItem
        {
            get { return _clarityItem; }
        }
        public Boolean IsActiveItem
        {
            get { return _isActiveItem; }
        }

        #endregion

        #region Constructors

        public ClarityItemRenderEventArgs(Graphics g, Form clarityForm, Form primaryClarityForm, ClarityItem item, Boolean isActiveItem)
            : base(g, clarityForm, primaryClarityForm)
        {
            _clarityItem = item;
            _isActiveItem = isActiveItem;
        }

        #endregion
    }
}