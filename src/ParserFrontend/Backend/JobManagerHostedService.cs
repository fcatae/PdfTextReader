using Microsoft.Extensions.Hosting;
using ParserFrontend.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ParserFrontend.Backend
{
    public class JobManagerHostedService : IHostedService
    {
        private JobManager _jobManager;

        public JobManagerHostedService(JobManager jobManager)
        {
            this._jobManager = jobManager;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while(!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await _jobManager.MessageLoopAsync();
                }
                catch
                {

                }
                await Task.Delay(10000);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
