using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var tasks = new List<Task>();

            for (var i = 0; i < 5; i++)
            {
                string user = $"User{i}";
                tasks.Add(Task.Run(async () => await TestMethod(user)));
            }

            //Assert.IsTrue(Task.WaitAll(tasks.ToArray(), 10000));
            Console.WriteLine($"IsTrue: " + Task.WaitAll(tasks.ToArray(), 10000));
            Console.ReadLine();
        }

        private static async Task TestMethod(string user)
        {

            using (var client = new TcpClient())
            {
                await client.ConnectAsync("127.0.0.1", 5050);

                using (var stream = client.GetStream())
                {
                    var sw = new StreamWriter(stream);
                    sw.AutoFlush = true;
                    var sr = new StreamReader(stream);
                    Console.WriteLine($"Request: Login: {user}");
                    await sw.WriteLineAsync($"Login: {user}");
                    var response = await sr.ReadLineAsync();
                    Console.WriteLine($"Response: " + response + $" [{Thread.CurrentThread.ManagedThreadId}]");
                    //await Task.Delay(3000);

                    Console.WriteLine($"Request: \"{user}: Send message blah blah blah\"");
                    await sw.WriteLineAsync($"{user}: Send message blah blah blah");

                    while(client.Connected)
                    {
                        response = await sr.ReadLineAsync();
                        Console.WriteLine($"{user} get response: " + response + $" [{Thread.CurrentThread.ManagedThreadId}]");
                    }

                }
            }
        }
    }
}
