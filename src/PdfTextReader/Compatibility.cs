using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader
{
    static class Compatibility
    {
        public static IEnumerable<T> TakeLast<T>(this List<T> list, int end)
        {
            int start = list.Count - end;
            return list.GetRange(start, end);
        }
    }
}
