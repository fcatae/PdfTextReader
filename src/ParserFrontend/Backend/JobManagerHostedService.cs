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

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var tasks = Enumerable
                .Range(0, 4)
                .Select(async numWorkers =>
                {
                    int timeout = 10000;
                    await RunAsync(timeout, cancellationToken).ConfigureAwait(false);
                })
                .ToArray();

            return Task.WhenAll(tasks);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task RunAsync(int timeout, CancellationToken cancellationToken)
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
                await Task.Delay(timeout);
            }
        }
    }
}
