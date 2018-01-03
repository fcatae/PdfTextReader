using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Lucas_Testes.Helpers
{
    public class StructureItem : LineItem
    {
        public StructureItem(List<MainItem> items) : base(items)
        {
        }

        static bool AreInSameStructure(LineItem i1, LineItem i2)
        {
            var i1b = i1.GetRectangle().GetBottom();
            var i1y = i1.GetRectangle().GetY();
            var i1ll = i1.GetLL();

            var i2b = i2.GetRectangle().GetBottom();
            var i2t = i2.GetRectangle().GetTop();
            var i2y = i2.GetRectangle().GetY();
            var i2ll = i2.GetLL();

            //Quebra de linha = 9.09
            float MarginLineY = 10;
            var fsize1 = i1.fontSize;
            var fsize2 = i2.fontSize;
            var diff = i1.GetRectangle().GetBottom() - i2.GetRectangle().GetBottom();

            //if (i1.GetRectangle().GetBottom() - i2.GetRectangle().GetBottom() > MarginLineY)
            //    return false;
            if (i1.GetRectangle().GetBottom() - i2.GetRectangle().GetBottom() < fsize2)
                return true;
            else if (i1.margin != Margin.CENTER && i2.isParagraph)
                return true;
            else if (i1.isSignture && i2.isSignture)
                return true;
            else if ((i1.isSignture && i1.margin != Margin.CENTER) || (i2.isSignture && i2.margin != Margin.CENTER))
                return false;
            else if ((i1.isUpperLetter && i2.isUpperLetter) && (i1.fontSize == i2.fontSize) && (i1.fontName == i2.fontName))
                return true;
            else if ((i1.margin == Margin.CENTER && i1.isParagraph == false) || (i2.margin == Margin.CENTER && i2.isParagraph == false))
                return false;
            //else if (!i1.GetColor().Equals(i2.GetColor()))
            //    return false;
            //else if (i2.GetRectangle().GetLeft() - i1.GetRectangle().GetLeft() >= MainItem.itemPositionTolerance)
            //    return false;
            return true;
        }

        public static List<StructureItem> GetStructures(List<LineItem> items, List<BlockSet> _list = null)
        {
            List<StructureItem> structures = new List<StructureItem>();
            List<MainItem> structure = new List<MainItem>();

            List<MainItem> ItemsInBlockSets;


            foreach (var b in _list)
            {
                ItemsInBlockSets = new List<MainItem>();
                foreach (var i in items)
                {
                    if (LineItem.IsInsideBlock(i, b))
                    {
                        ItemsInBlockSets.Add(i);
                    }
                }
                //ItemsInBlockSets.Sort();
                foreach (var line in ItemsInBlockSets)
                {
                    if (structure.Count == 0)
                    {
                        structure.Add(line);
                        continue;
                    }
                    if (AreInSameStructure((LineItem)structure[structure.Count - 1], (LineItem)line))
                    {
                        structure.Add(line);
                    }
                    else
                    {
                        structures.Add(new StructureItem(structure));
                        structure = new List<MainItem>
                    {
                        line
                    };
                    }
                }
                if (structure.Count > 0)
                    structures.Add(new StructureItem(structure));
            }

            return structures;
        }
    }
}
