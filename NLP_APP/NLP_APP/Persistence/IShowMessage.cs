using System;
using System.Collections.Generic;
using System.Text;

namespace NLP_APP.Persistence
{    
    public interface IShowMessage
    {
        void Show(string sMessage, bool bLongMess = true);
    }
}
