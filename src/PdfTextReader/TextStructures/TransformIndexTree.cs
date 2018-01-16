using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.TextStructures
{
    class TransformIndexTree
    {
        Dictionary<Type, ITransformIndex> _indexes = new Dictionary<Type, ITransformIndex>();

        public void AddRef<TI,TO>(TransformIndex<TI,TO> index)
        {
            // currently does not allow duplicates
            _indexes.Add(typeof(TO), index);
        }

        ITransformIndex Get(Type objType)
        {
            return (ITransformIndex)_indexes[objType];
        }

        public int FindPageStart<TO>(TO instance)
        {
            TextLine textLine = null;
            object searchObject = instance;

            while(searchObject != null)
            {
                var index = Get(instance.GetType());

                textLine = searchObject as TextLine;

                if (textLine != null)
                    break;

                searchObject = index.GetStart(searchObject);
            }

            if (textLine == null)
                throw new InvalidOperationException();

            return textLine.PageInfo.PageNumber;
        }
    }
}
