using System;
using System.Collections.Generic;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.PDFCore
{
    class ReplaceCharacters : IProcessBlock
    {
        const float MINIMUM_CHARACTER_OVERLAP = .5f;

        public BlockPage Process(BlockPage page)
        {
            IBlock last = null;
            var result = new BlockPage();

            foreach (var block in page.AllBlocks)
            {
                string ttt = block.GetText();

                if (last != null)
                {
                    // are the same lines
                    if (HasIntersectionH(block, last))
                    {
                        bool isBackspace = CheckBackspace(last, block);
                        
                        if (isBackspace)
                        {
                            float endofblock = block.GetX() + block.GetWidth();
                            float endoflast = last.GetX() + last.GetWidth();
                            float diff = endofblock - endoflast;

                            string lastText = last.GetText();
                            string curText = block.GetText();

                            if ((lastText.Length == 1) && (curText.Length == 1) && (lastText != " "))
                            {
                                string text = lastText + curText;
                                string replaceText = null;

                                if (text == "o-")
                                {
                                    // Unicode 186 (0xba) = º
                                    replaceText = "\xba";
                                }

                                if (replaceText != null)
                                {
                                    ((Block)last).Text = replaceText;

                                    // do not set last: ignore the current block
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            bool isBackspace2 = CheckBackspaceAgain(last, block);
                            bool isSingleChar = block.GetText().Length == 1;

                            if (isBackspace2 && isSingleChar)
                            {
                                char ch = block.GetText()[0];

                                // ignore spaces
                                if(ch == ' ')
                                    continue;

                                string text = last.GetText();

                                if (String.IsNullOrEmpty(text))
                                    PdfReaderException.Throw("text should not be empty");

                                char lastTextChar = text[text.Length - 1];

                                if (IsAcentoDuplo(lastTextChar))
                                    continue;

                                // ignora os acentos composto por caracteres (bug #100)
                                int idxLast = GetIdxVogal(lastTextChar);
                                int idxNext = GetIdxAcento(ch);

                                string newChar = GetVogalAcento(idxLast, idxNext);

                                if ( idxLast >=0 && idxNext >= 0 && newChar != null)
                                {                                    
                                    string replaceText = text.Substring(0, text.Length - 1) + newChar;
                                    ((Block)last).Text = replaceText;
                                    ((Block)block).Text = " ";

                                    // do not set last: ignore the current block
                                    continue;
                                }

                                PdfReaderException.Throw("Unknown character");
                            }
                            
                            if(isBackspace2)
                                PdfReaderException.Throw("It will fail the GroupLine check");
                        }
                    }

                    // defer adding the current item
                    // only add the last block
                    result.Add(last);
                }

                last = block;
            }

            // always add the last block
            if (last != null)
                result.Add(last);

            return result;
        }

        int GetIdxVogal(char ch)
        {
            switch (ch)
            {
                case 'a': return 0;
                case 'e': return 1;
                case 'i': return 2;
                case 'o': return 3;
                case 'u': return 4;
            }

            return -1;
        }

        int GetIdxAcento(char ch)
        {
            switch (ch)
            {
                case '~': return 0;
                case '`': return 1;
                case '´': return 2;
            }

            return -1;
        }
        
        readonly string[,] tabelaAcentuacao = new string[,] {
            { "ã", "e~", "i~", "õ", "u~" },
            { "à", "è", "ì", "ò", "ù" },
            { "á", "é", "í", "ó", "ú" }
        };
        readonly string tabelaAcentuacaoFlat =
            "ãõ" +
            "àèìòù" +
            "áéíóú";


        string GetVogalAcento(int idxVogal, int idxAcento)
        {
            if (idxVogal < 0 || idxVogal > tabelaAcentuacao.GetLength(1))
                return null;

            if (idxAcento < 0 || idxAcento > tabelaAcentuacao.GetLength(0))
                return null;

            return tabelaAcentuacao[idxAcento, idxVogal];    
        }

        bool IsAcentoDuplo(char ch1)
        {
            return (tabelaAcentuacaoFlat.Contains(ch1.ToString()));
        }

        bool HasIntersectionH(IBlock a, IBlock b)
        {
            float aH1 = a.GetH();
            float aH2 = a.GetH() + a.GetHeight();
            float bH1 = b.GetH();
            float bH2 = b.GetH() + b.GetHeight();

            return ((aH1 <= bH1) && (aH2 >= bH1)) || ((aH1 <= bH2) && (aH2 >= bH2));
        }
        
        bool CheckBackspace(IBlock line, IBlock block)
        {
            float lineX = line.GetX() + line.GetWidth()*(1-MINIMUM_CHARACTER_OVERLAP);
            float wordX = block.GetX();

            return (wordX < lineX);
        }

        bool CheckBackspaceAgain(IBlock line, IBlock block)
        {
            float lineX = line.GetX() + line.GetWidth();
            float wordX = block.GetX() + block.GetWidth();

            return (wordX < lineX);
        }
    }
}
