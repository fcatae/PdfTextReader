using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace PdfTextReader.TextStructures
{
    class AnalyzeTreeStructure : ILogStructure2<TextSegment>
    {
        ITransformIndexTree _index;
        Stack<string> _currentTree = new Stack<string>();
        int _structureId = 0;
        
        public void Init(ITransformIndexTree index)
        {
            if (index == null)
                throw new ArgumentNullException();

            _index = index;
        }

        public void Log(TextWriter input, TextSegment artigo)
        {
            var titles = artigo.Title;
            int p = (_index != null) ? _index.FindPageStart(artigo) : -1;

            while (_currentTree.Count > titles.Length)
            {
                _currentTree.Pop();
            }

            int nivel = _currentTree.Count - 1;

            while (_currentTree.Count > 0)
            {
                if (titles[nivel--].Text == _currentTree.Peek())
                    break;

                _currentTree.Pop();
            }

            while (titles.Length > _currentTree.Count)
            {
                nivel = _currentTree.Count;

                string titleText = titles[nivel].Text;

                // adjust the hierarchy
                for (int j = 0; j < nivel; j++)
                    input.Write("    ");

                // print
                string optPageInfo = "";

                _currentTree.Push(titleText);

                if (titles.Length == _currentTree.Count)
                    optPageInfo = $" (Page {p}, ID={_structureId})";

                input.WriteLine(titleText.Replace("\n", " ") + optPageInfo);
            }

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
