using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Clarity
{
    public interface IClarityProvider
    {
        #region Methods
        
        List<IClarityProvider> GetChildClarityProviders(Int32 state);
        List<ClarityItem> GetClarityItems(Int32 state);
        //Boolean IncludeNonClientArea { get; }

        #endregion
    }
}