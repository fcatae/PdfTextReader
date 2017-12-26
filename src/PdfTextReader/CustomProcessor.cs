using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;

namespace PdfTextReader
{
    public class CustomProcessor
    {
        double margin;
        float header = 90;
        float footer = 15;
        int ColumnNumber;
        float BreakLine = 7.09f;
        float Paragraph = 28;
        List<Block> localList;
        float WordSpace = 5;
        float PageHeight;
        float PageWidth;
        PdfPage Page;
        PdfCanvas Canvas;

        public CustomProcessor(List<Block> list, PdfPage page, PdfCanvas canvas)
        {
            this.localList = list;
            this.Page = page;
            this.PageHeight = page.GetPageSize().GetHeight();
            this.PageWidth = page.GetPageSize().GetWidth();
            this.Canvas = canvas;
        }

        public void BuildLines()
        {
            //Variáveis locais
            bool Next = true;
            bool NextLine = false;
            float endblock = 0;
            StringBuilder FullText = new StringBuilder();
            BlockLine bline;
            List<Block> BlocksInLine = new List<Block>();
            List<BlockLine> AllLines = new List<BlockLine>();

            //Iniciando a primeira coluna
            ColumnNumber = 1;
           
            //Pegando a primeira linha
            float MAX_Y = localList.Where(b => b.GetH() < PageHeight - header).Max(b => b.GetH());
            
            //Pegando o início da primeira linha da coluna
            float MIN_X_InLine = localList.Where(b => b.GetH() == MAX_Y).Min(b => b.GetX());
            margin = Math.Round(localList.Min(b => b.X), MidpointRounding.AwayFromZero);

            //Pegando a referência da menor linha, retirando o rodapé
            float MIN_Y = localList.Where(b => b.GetH() > footer).Min(b => b.GetH());


            /*
             * AREA PARA TESTES
             * 
             */


            //IEnumerable<Block> aaa = localList.Where(b => b.Text.Contains("DELEG"));
            //if (aaa != null)
            //{

            //}



            /*
            * AREA PARA TESTES
            * 
            */


            while (Next)
            {
                bline = new BlockLine();

                //Verifico se houve quebra de linha
                if (NextLine)
                { 
                    BlocksInLine.Clear();
                    Block firstBlockset = localList.Where(b => b.GetH() == MAX_Y).FirstOrDefault();
                    //Acabou a primeira coluna
                    if (firstBlockset.GetH() < MIN_Y)
                    {
                        ColumnNumber += 1;
                        MAX_Y = localList.Where(b => b.GetH() < PageHeight - header && b.GetX() > 290 && b.GetX() < 390).Max(b => b.GetH());
                        MIN_X_InLine = localList.Where(b => b.GetH() == MAX_Y && b.GetX() > 290).Min(b => b.GetX());
                    }
                    else
                    {
                        MAX_Y = localList.Where(b => b.GetH() < firstBlockset.GetH() - 1 && b.GetX() < 290).Max(b => b.GetH());
                        MIN_X_InLine = localList.Where(b => b.GetH() == MAX_Y).Min(b => b.GetX());
                    }
                    FullText.Clear();
                }

                //Pegando o primeiro stream de texto (BLOCK) da linha
                Block firstBlock = localList.Where(b => b.GetH() == MAX_Y && b.GetX() == MIN_X_InLine).FirstOrDefault();

                //Adicionando o Block à lista local para formar a linha
                BlocksInLine.Add(firstBlock);

                //Adicionando o ponto inicial da linha (primeiro x do primeiro block)
                if (bline.X == 0)
                {
                    bline.X = firstBlock.X;
                    bline.H = firstBlock.H;
                }

                //Montando o texto da linha
                FullText.Append(firstBlock.Text + " ");

                //Pegando o final do primeiro bloco para buscar se existe um bloco adjacente
                endblock = firstBlock.GetX() + firstBlock.GetWidth();

                //O tamanho da linha será sempre o X final do último bloco menos o X inicial do primeiro bloco.
                bline.Width = endblock - bline.X;

                //Verificando se existe um bloco adjacente com margem de 5px definido pela variável WordSpace
                //Para tal, o bloco deve estar no mesmo eixo Y mas com o X inicial adjacente ao X final do bloco anterior
                Block NextBlock = localList.Where(b => b.GetX() > endblock - WordSpace && b.GetX() < endblock + WordSpace && b.GetH() == firstBlock.GetH() && b != firstBlock).FirstOrDefault();

                while (NextBlock != null)
                {
                    BlocksInLine.Add(NextBlock);
                    FullText.Append(NextBlock.GetText() + " ");
                    endblock = NextBlock.GetX() + NextBlock.GetWidth();
                    //Atualizando o tamanho da linha a cada iteração
                    bline.Width = endblock - bline.X;
                    NextBlock = localList.Where(b => b.GetX() > endblock - WordSpace && b.GetX() < endblock + WordSpace && b.GetH() == NextBlock.GetH() && b != NextBlock).FirstOrDefault();
                }


                //Enquanto a posição Y for maior que o rodapé, estamos pegando texto 
                if (firstBlock.GetH() != MIN_Y)
                {
                    Next = true;
                }
                else
                {
                    Next = false;
                }

                NextLine = true;

                //Desenhamos a linha em vermelho apenas como recurso visual
                DrawRectLine(firstBlock, endblock);

                //Processo a linha construindo o objeto BlockLine
                ProcessLine(bline, FullText.ToString(), BlocksInLine);

                //Adiciono o BlockLine no array
                if (bline.GetH() > MIN_Y)
                {
                    AllLines.Add(bline);
                }
            }
            var a = AllLines;
        }

        private void ProcessLine(BlockLine bline, string line, List<Block> list)
        {
            bline.Text = line;

            bline.B = list.Max(b => b.B);
            bline.FontFullName = list.FirstOrDefault().FontFullName;
            bline.FontName = list.FirstOrDefault().FontName;
            bline.FontSize = list.FirstOrDefault().FontSize;
            bline.FontStyle = list.FirstOrDefault().FontStyle;
            bline.Height = list.FirstOrDefault().Height;
            bline.Lower = list.FirstOrDefault().Lower;

            //Page Properties
            bline.NumberOfColumns = Math.Floor((PageWidth - (margin * 2)) / bline.Width);
            bline.ColumnSize = (PageWidth - (margin * 2)) / bline.NumberOfColumns;

            bline.Column = ColumnNumber;
            if (bline.ColumnSize > bline.Width)
            {
                bline.ColumnSpan = 1;
            }
            if (bline.Width >= bline.ColumnSize && bline.Width <= (bline.ColumnSize * 2))
            {
                bline.ColumnSpan = 2;
            }
            if (bline.Width > (bline.ColumnSize * 2))
            {
                bline.ColumnSpan = 3;
            }

            //Verifica se a frase está toda em maiúsculo
            if (line.ToUpper() == line)
            {
                bline.UpperCase = true;
            }
        }

        void DrawRectLine(Block b, float end)
        {
            DrawRect(Canvas, b, GetStyle(b), end);
        }

        private enum Style
        {
            MINISTERIO,
            DEPARTAMENTO,
            SETOR,
            TITULO,
            TEXTO,
            NOTFOUND
        }

        void DrawRect(PdfCanvas canvas, Block bset, Style style, float end)
        {
            switch (style)
            {
                case Style.DEPARTAMENTO:
                    canvas.SetStrokeColor(ColorConstants.MAGENTA);
                    canvas.Rectangle(bset.GetX(), bset.GetH(), bset.GetWidth(), bset.GetHeight());
                    canvas.Stroke();
                    break;
                case Style.MINISTERIO:
                    canvas.SetStrokeColor(ColorConstants.CYAN);
                    canvas.Rectangle(bset.GetX(), bset.GetH(), bset.GetWidth(), bset.GetHeight());
                    canvas.Stroke();
                    break;
                case Style.SETOR:
                    canvas.SetStrokeColor(ColorConstants.ORANGE);
                    canvas.Rectangle(bset.GetX(), bset.GetH(), bset.GetWidth(), bset.GetHeight());
                    canvas.Stroke();
                    break;
                case Style.TITULO:
                    canvas.SetStrokeColor(ColorConstants.PINK);
                    canvas.Rectangle(bset.GetX(), bset.GetH(), bset.GetWidth(), bset.GetHeight());
                    canvas.Stroke();
                    break;
                case Style.TEXTO:
                    Canvas.SetStrokeColor(ColorConstants.RED);
                    Canvas.Rectangle(bset.GetX(), bset.GetH(), end - bset.GetX(), bset.Height - bset.Lower);
                    Canvas.Stroke();
                    break;
                case Style.NOTFOUND:
                    break;
                default:
                    break;
            }
        }


        Style GetStyle(Block b)
        {
            if (b.FontSize > 10)
            {
                return Style.MINISTERIO;
            }
            if (b.FontSize > 9 && b.FontName.ToLower().Contains("bold"))
            {
                return Style.DEPARTAMENTO;
            }

            if (b.FontSize > 9)
            {
                return Style.SETOR;
            }

            if (b.FontName.ToLower().Contains("bold"))
            {
                return Style.TITULO;
            }

            //Text Parameters
            if (b.FontSize < 9 && b.FontSize > 4)
            {
                return Style.TEXTO;
            }

            return Style.NOTFOUND;
        }
    }

    public class BlockLine : Block
    {
        public BlockLine()
        {
            this.X = 0;
            this.UpperCase = false;
            this.Paragraph = false;
            this.Center = false;
        }
        public bool Paragraph { get; set; }
        public bool Center { get; set; }
        public bool UpperCase { get; set; }
        public double NumberOfColumns { get; set; }
        public double ColumnSize { get; set; }
        public int Column { get; set; }
        public int ColumnSpan { get; set; }
        public int Row { get; set; }
    }
}