using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ParserRun
{
    class Config
    {
        private readonly IConfigurationRoot _config;

        public Config(string[] args)
        {
            _config = new ConfigurationBuilder()
                            .AddUserSecrets<Config>(optional: true)
                            .AddEnvironmentVariables("PDFPARSER")
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
