using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace PdfTextReader.TextStructures
{
    class AnalyzePageInfo<T> : ILogStructure2<T>
    {
        ITransformIndexTree _index;
        int _structureId = 0;
        
        public void Init(ITransformIndexTree index)
        {
            if (index == null)
                throw new ArgumentNullException();

            _index = index;
        }

        public void Log(TextWriter input, T instance)
        {
            int page = _index.FindPageStart(instance);

            input.WriteLine($"Page {page}: {_structureId} [{instance.ToString().Replace("\n", " ")}]");

            _structureId++;
        }

        public void StartLog(TextWriter input)
        {
        }

        public void EndLog(TextWriter input)
        {
        }

    }
}
