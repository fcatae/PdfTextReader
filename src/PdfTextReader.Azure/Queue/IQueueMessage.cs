using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Azure.Queue
{
    public interface IQueueMessage
    {
        string Content { get; }
        void Done();
    }

}
