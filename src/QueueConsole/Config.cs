using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace QueueConsole
{
    class Config
    {
        private readonly IConfigurationRoot _config;

        public Config(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("DOTNETCORE_ENVIRONMENT");

            _config = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json",true,true)
                            .AddJsonFile($"appsettings.{env}.json", true, true)
                            .AddEnvironmentVariables()
                            .AddCommandLine(args)
                            .Build();
        }

        [DebuggerHidden]
        public string Get(string configName)
        {
            var value = _config[configName];

            if (value == null)
                throw new NotConfigured(configName);

            return value;
        }


        class NotConfigured : Exception
        {
            public readonly string Name;

            public NotConfigured(string name) : base($"configuration '{name}' not found")
            {
                Name = name;
            }
        }
    }
}
