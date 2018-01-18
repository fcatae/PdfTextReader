using System;
using System.Collections.Generic;
using System.Text;

namespace Validator
{
    interface IRunner
    {
        void Run(File file, string outputname);
    }
}
