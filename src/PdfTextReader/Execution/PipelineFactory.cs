using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Execution
{
    class PipelineFactory
    {
        static PipelineFactory g_singleton = new PipelineFactory();

        static public T Create<T>()
            where T : new()
        {
            return g_singleton.CreateInstance<T>();
        }

        Dictionary<Type, object> _dictionary = new Dictionary<Type, object>();

        public T CreateInstance<T>()
            where T: new()
        {
            var objectType = typeof(T);

            if ( !_dictionary.ContainsKey(objectType) )
            {
                _dictionary[objectType] = new T();
            }

            object instance = _dictionary[objectType];
            
            return (T)instance;
        }
    }
}
