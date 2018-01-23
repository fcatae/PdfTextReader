using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.TextStructures
{
    class TransformIndexTree : ITransformIndexTree
    {
        List<ITransformIndex> _indexes = new List<ITransformIndex>();
        List<Type> _indexOutputType = new List<Type>();

        public void AddRef<TI,TO>(TransformIndex<TI,TO> index)
        {
            _indexes.Add(index);
            _indexOutputType.Add(typeof(TO));
        }

        ITransformIndex GetIndex(Type objType)
        {
            int index_end = _indexes.Count - 1;

            for (int i=index_end; i>=0; i--)
            {
                if (_indexOutputType[i] == objType)
                    return _indexes[i];
            }

            throw new InvalidOperationException();
        }

        public int FindIndex<T>(T instance)
        {
            int index_end = _indexes.Count - 1;

            object last_instance = instance;
            for (int i = index_end; i >= 0; i--)
            {
                if (last_instance is T)
                    break;

                Type objType = last_instance.GetType();

                // validate type
                if (_indexOutputType[i] != objType)
                    continue;

                var index = _indexes[i];

                object childInstance = index.GetStart(last_instance);

                last_instance = childInstance;
            }

            var text = last_instance as TextLine;

            if (text == null)
                throw new InvalidOperationException();

            return text.PageInfo.PageNumber;
        }

        public int FindPageStart<T>(T instance)
        {
            if (_indexOutputType[0] != typeof(TextLine))
                throw new InvalidOperationException("requires CreateTextLineIndex pipeline");

            int index_end = _indexes.Count - 1;

            while(index_end >= 0)
            {
                if (_indexOutputType[index_end] == typeof(T))
                    break;
                index_end--;
            }

            if (index_end < 0)
                throw new InvalidOperationException();
            
            if (_indexOutputType[index_end] != typeof(T))
                throw new InvalidOperationException();

            int last_instanceId = _indexes[index_end].GetObjectId(instance);
            TextLine line = null;

            for (int i = index_end; i >= 0; i--)
            {
                var index = _indexes[i];

                if (_indexOutputType[i] == typeof(TextLine))
                {
                    line = (TextLine)index.GetInstance(last_instanceId);
                    break;
                }

                int childInstanceId = index.GetStartId(last_instanceId);

                last_instanceId = childInstanceId;
            }
            
            if (line == null)
                throw new InvalidOperationException();

            return line.PageInfo.PageNumber;
        }

        public int FindPageEnd<T>(T instance)
        {
            if (_indexOutputType[0] != typeof(TextLine))
                throw new InvalidOperationException("requires CreateTextLineIndex pipeline");

            int index_end = _indexes.Count - 1;

            if (_indexOutputType[index_end] != typeof(T))
                throw new InvalidOperationException();

            int last_instanceId = _indexes[index_end].GetObjectId(instance);
            TextLine line = null;

            for (int i = index_end; i >= 0; i--)
            {
                var index = _indexes[i];

                if (_indexOutputType[i] == typeof(TextLine))
                {
                    line = (TextLine)index.GetInstance(last_instanceId);
                    break;
                }

                int childInstanceId = index.GetEndId(last_instanceId);

                last_instanceId = childInstanceId;
            }

            if (line == null)
                throw new InvalidOperationException();

            return line.PageInfo.PageNumber;
        }

        public int FindPageStartOld(object instance)
        {
            int index_end = _indexes.Count - 1;

            int last_instanceId = 0;
            object last_instance = instance;
            for (int i = index_end; i >= 0; i--)
            {
                if (last_instance is TextLine)
                    break;

                Type objType = last_instance.GetType();

                // validate type
                if (_indexOutputType[i] != objType)
                    throw new InvalidOperationException();

                var index = _indexes[i];

                int childInstanceId = index.GetStartId(last_instanceId);
                object childInstance = index.GetStart(last_instance);

                last_instance = childInstance;
            }

            var text = last_instance as TextLine;

            if (text == null)
                throw new InvalidOperationException();

            return text.PageInfo.PageNumber;
        }

        public int FindPageStart2<TO>(TO instance)
        {
            TextLine textLine = null;
            object searchObject = instance;

            while(searchObject != null)
            {
                var index = GetIndex(instance.GetType());
                
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
