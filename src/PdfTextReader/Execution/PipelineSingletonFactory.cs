using System;
using System.Collections.Generic;
using System.Text;
using PdfTextReader.Base;
using Autofac;
using System.Reflection;

namespace PdfTextReader.Execution
{
    // TODO: move to pipeline page
    // It cannot be a singleton because it will break when 
    // processing multiple pages in a single execution

    class PipelineSingletonAutofacFactory : IDisposable
    {
        Dictionary<Type, object> _dictionary = new Dictionary<Type, object>();
        IContainer _container;
        static IContainer g_container;
        ILifetimeScope _scope;

        static PipelineSingletonAutofacFactory()
        {
            var builder = new ContainerBuilder();

            //builder.RegisterAssemblyModules(Assembly.GetExecutingAssembly()).;
            var currentAssembly = Assembly.GetExecutingAssembly();

            builder.RegisterAssemblyTypes(currentAssembly)
                //.Where(t => t.IsAssignableTo<IProcessBlock>())
                .InstancePerLifetimeScope();

            g_container = builder.Build();
        }

        public PipelineSingletonAutofacFactory()
        {
            _scope = g_container.BeginLifetimeScope();
        }

        public T CreateInstance<T>()
        {
            var objectType = typeof(T);

            if ( !_dictionary.ContainsKey(objectType) )
            {
                _dictionary[objectType] = _scope.Resolve<T>();
            }

            object instance = _dictionary[objectType];
            
            return (T)instance; 
        }

        public void Dispose()
        {
            if( _scope != null )
            {
                _scope.Dispose();
                _scope = null;
            }
        }
    }

    class PipelineSingletonFactory
    {
        Dictionary<Type, object> _dictionary = new Dictionary<Type, object>();

        public T CreateInstance<T>()
            where T : new()
        {
            var objectType = typeof(T);

            if (!_dictionary.ContainsKey(objectType))
            {
                _dictionary[objectType] = new T();
            }

            object instance = _dictionary[objectType];

            return (T)instance;
        }
    }
}
