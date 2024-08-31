using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AdditionalAccountCreation.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<ProcessProfiling> _logger;
        private readonly ProcessProfiling _process;
        private readonly ConnectionManager _connection;



        public Worker(ILogger<ProcessProfiling> logger, ConnectionManager connectionManager, ProcessProfiling process)
        {
            _logger = logger;
            _connection = connectionManager;
            _process = process;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                string runtime = _connection.RunTime;

                string timeNow = DateTime.Now.ToString("t");

                if (runtime == timeNow)
                {
                    string myHost = Dns.GetHostName();
                    var myIPListObject = Dns.GetHostByName(myHost).AddressList;
                    string myIP = Dns.GetHostByName(myHost).AddressList[0].ToString();
                    string IPList = _connection.AllowedIP;
                    var allowedIPs = IPList.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
                    List<string> myIPList = new List<string>();

                    foreach (var IPs in myIPListObject)
                    {
                        myIPList.Add(IPs.ToString());
                    }

                    string processName = Process.GetCurrentProcess().ProcessName;
                    Process[] currentRunningProcessess = Process.GetProcessesByName(processName);
                    int currentProcessCount = currentRunningProcessess.Length;

                    if (currentProcessCount > 1)
                    {
                        Console.WriteLine("Another instance of the process is currently running");
                        _logger.LogInformation($"{DateTime.Now:dd-MM-yyyy HH:mm:ss} () :  Another instance of the process is currently running");
                        Environment.Exit(0);
                    }
                    else if (!allowedIPs.Any(myIPList.Contains))
                    {
                        Console.WriteLine("Machine not allowed to use this service");
                        _logger.LogInformation($"{DateTime.Now:dd-MM-yyyy HH:mm:ss} () :  Machine not allowed to use this service");
                        Environment.Exit(0);
                    }

                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    _process.ProcessNewAccount();
                   
                }
                else

                {
                    _logger.LogInformation("Worker Scheduled to Run at: {time}", runtime);
                }
                await Task.Delay(Convert.ToInt32(_connection.DelayTime), stoppingToken);

            }
        }
    }
}
