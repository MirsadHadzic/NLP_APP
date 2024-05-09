using System;
using System.Collections.Generic;
using System.Text;

namespace NLP_APP.Persistence
{
    public interface IClassify
    {
        string GetClassify(string inputText);
    }
}
