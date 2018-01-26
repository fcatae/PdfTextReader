using System;

namespace ParserRun
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Parser Run");

            // DEV: configure the secrets
            //
            //   dotnet user-secrets set exemplo abc
            //
            var config = new Config(args);

            Console.WriteLine(config.Get("exemplo1"));
        }
    }
}
