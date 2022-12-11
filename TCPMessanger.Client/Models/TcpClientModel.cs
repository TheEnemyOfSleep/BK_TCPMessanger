using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace TCPMessanger.Client.Models
{
    public class TcpClientModel
    {
        private TcpClient _client;
        private Stream _stream;
        private CancellationTokenSource _tokenSource;
        private CancellationToken _token;
        private StreamReader _sr;
        private StreamWriter _sw;

        public TcpClient Client
        {
            get => _client;
        }

        public event EventHandler<DataReceivedEventArgs> DataReceived;

        public class DataReceivedEventArgs : EventArgs
        {
            public string Data { get; private set; }

            public DataReceivedEventArgs(string data)
            {
                Data = data;
            }
        }

        public TcpClientModel()
        {
            _client = new TcpClient();
        }

        private async Task Close()
        {
            await Task.Yield();
            if (_client != null)
            {

                _client.Close();
                _client.Dispose();
                _client = null;
            }
            if (_stream != null)
            {
                _stream.Dispose();
                _stream = null;
            }
        }

        private async Task CloseIfCanceled(CancellationToken token, Action onClosed = null)
        {
            if (token.IsCancellationRequested)
            {
                await Close();
                if (onClosed != null)
                    onClosed();
                token.ThrowIfCancellationRequested();
            }
        }

        public async Task ConnectClient(IPAddress ipAddress, int port, CancellationToken? token = null)
        {
            _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(token ?? new CancellationToken());
            _token = _tokenSource.Token;

            try
            {
                await Close();
                _token.ThrowIfCancellationRequested();
                _client = new TcpClient();
                _token.ThrowIfCancellationRequested();
                await _client.ConnectAsync(ipAddress, port);
                await CloseIfCanceled(_token);
                
                _stream = _client.GetStream();
                _sr = new StreamReader(_stream);
                _sw = new StreamWriter(_stream);
                _sw.AutoFlush = true;
                await CloseIfCanceled(_token);

                var receiveTast = Task.Run(async () => {
                    while (true)
                    {
                        var result = await _sr.ReadLineAsync();
                        DataReceived?.Invoke(this, new DataReceivedEventArgs(result));
                    }
                });
            }
            catch(Exception)
            {
                CloseIfCanceled(_token).Wait();
            }
        }

        public async Task SendStringAsync(string message)
        {
            try
            {
                await _sw.WriteLineAsync(message);
                await _sw.FlushAsync();
            }
            catch (IOException e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
