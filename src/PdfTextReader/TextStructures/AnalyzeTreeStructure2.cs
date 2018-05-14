using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace PdfTextReader.TextStructures
{
    class AnalyzeTreeStructure2 : ILogStructure2<TextSegment>
    {
        ITransformIndexTree _index;
        Stack<string> _currentTree = new Stack<string>();
        int _structureId = 0;
        int _lastPid = -1;
        int _lastPage = -1;

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

            //int nivel = _currentTree.Count - 1;
            //while (_currentTree.Count > 0)
            //{
            //    if (titles[nivel--].Text == _currentTree.Peek())
            //        break;

            //    _currentTree.Pop();
            //}

            // find difference
            int nivel = 0;
            var currentArrayTree = _currentTree.Reverse().ToArray();
            while(nivel < _currentTree.Count)
            {
                if (titles[nivel].Text != currentArrayTree[nivel])
                    break;
                nivel++;
            }
            // end find diff
            while(_currentTree.Count > nivel)
            {
                _currentTree.Pop();
            }


            while (titles.Length > _currentTree.Count)
            {
                nivel = _currentTree.Count;

                string titleText = titles[nivel].Text;

                // separate the main titles
                if (nivel == 0)
                    input.WriteLine();

                // adjust the hierarchy
                for (int j = 0; j < nivel; j++)
                    input.Write("    ");

                // print
                string optPageInfo = "";

                _currentTree.Push(titleText);

                // reset pid
                if( _lastPage != p )
                {
                    _lastPid = 1;
                    _lastPage = p;
                }

                if (titles.Length == _currentTree.Count)
                {
                    optPageInfo = $" ((Page {p}, PID={_lastPage}:{_lastPid}, NUM={_structureId}))";
                    _lastPid++;
                }

                string idmateria = TitleWithHiddenIdMateria.GetIdMateria(titleText);
                string title = GetTitleText(titleText);

                if (idmateria != null)
                    optPageInfo += $"((IdMateria={idmateria}))";

                input.WriteLine( title + optPageInfo);
            }

            _structureId++;
        }

        string GetTitleText(string text)
        {
            string cleanText = TitleWithHiddenIdMateria.CleanIdMateria(text);

            return cleanText.Replace("\n", " ");
        }

        public void StartLog(TextWriter input)
        {
        }

        public void EndLog(TextWriter input)
        {
        }

    }
}
