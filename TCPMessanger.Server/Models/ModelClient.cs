using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPMessanger.Server.Models
{
    class ModelClient
    {
        public TcpClient Client { get; set; }
        public string Nickname { get; set; }

        public ModelClient(TcpClient client, string nickname)
        {
            Client = client;
            Nickname = nickname;
        }
    }
}
