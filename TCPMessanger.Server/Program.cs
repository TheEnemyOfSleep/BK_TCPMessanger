using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TCPMessanger.Server.Models;

namespace TCPMessanger.Server
{
    class Program
    {
        static IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5050);
        static TcpListener listener = new TcpListener(endpoint);
        static List<ModelClient> connectedClients = new List<ModelClient>();

        static async Task Main(string[] args)
        {
            using (var server = new TcpServer(endpoint))
            {
                server.DataReceived += async (sender, ev) => await OnDataReceived(sender, ev);
                var serverTask = server.StartAsync();

                await serverTask;
            }
        }

        private static async Task OnDataReceived(object sender, TcpServer.DataReceivedEventArgs ev)
        {
            TcpClient client = ev.Client;
            NetworkStream stream = client.GetStream();
            StreamWriter sw = new(stream);
            sw.AutoFlush = true;
            StreamReader sr = new(stream);
            ModelClient modelClient;

            while (true)
            {
                var line = await sr.ReadLineAsync();
                if (!String.IsNullOrWhiteSpace(line))
                {
                    string nickname = line.Replace("Login: ", "");
                    if (line.Contains("Login:") && !string.IsNullOrWhiteSpace(nickname))
                    {
                        if (connectedClients.FirstOrDefault(c => c.Nickname == nickname) is null)
                        {
                            modelClient = new ModelClient(client, nickname);
                            connectedClients.Add(modelClient);
                            await SendToAllClients($"User {nickname} connected");
                            Console.WriteLine($"New connected: {nickname}");
                            break;
                        }
                        else
                        {
                            await sw.WriteLineAsync("A user with this nickname already exists");
                            client.Client.Disconnect(false);
                        }
                    }
                }
            }

            while(true)
            {
                try
                {
                    var line = await sr.ReadLineAsync();
                    if (!String.IsNullOrWhiteSpace(line))
                    {
                        string stringLine = line.ToString();
                        await SendToAllClients(stringLine);
                        Console.WriteLine(line);
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    connectedClients.Remove(
                        connectedClients.FirstOrDefault(c => c.Nickname == modelClient.Nickname));
                    break;
                }
            }
        }

        private static async Task SendToAllClients(string message)
        {
            for(int i = 0; i < connectedClients.Count; i++)
            {
                try
                {
                    if(connectedClients[i].Client.Connected)
                    {
                        StreamWriter sw = new(connectedClients[i].Client.GetStream());
                        sw.AutoFlush = true;
                        await sw.WriteLineAsync($"{DateTime.Now.ToString("HH:mm tt")} - {message}");
                    }
                    else
                    {
                        Console.WriteLine($"{connectedClients[i].Nickname} disconnected");
                        connectedClients.RemoveAt(i);
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
