using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPMessanger.Server
{
    public class TcpServer: IDisposable
    {
        private readonly TcpListener _listener;
        private CancellationTokenSource _tokenSource;
        private CancellationToken _token;
        public event EventHandler<DataReceivedEventArgs> DataReceived;

        public class DataReceivedEventArgs : EventArgs
        {
            public TcpClient Client { get; private set; }

            public DataReceivedEventArgs(TcpClient client)
            {
                Client = client;
            }
        }

        public TcpServer(IPEndPoint endpoint)
        {
            _listener = new TcpListener(endpoint);
        }

        public async Task StartAsync(CancellationToken? token = null)
        {
            _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(token ?? new CancellationToken());
            _token = _tokenSource.Token;
            _listener.Start();
            Console.WriteLine("Server started...");

            try
            {
                while (!_token.IsCancellationRequested)
                {
                    await Task.Run(async () =>
                    {
                        while (true)
                        {
                            var tcpClientTask = _listener.AcceptTcpClientAsync();
                            var result = await tcpClientTask;
                            DataReceived?.Invoke(this, new DataReceivedEventArgs(result));
                        }

                    }, _token);
                }
            }
            finally
            {
                _listener.Stop();
            }
        }

        public void Stop()
        {
            _tokenSource?.Cancel();
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
