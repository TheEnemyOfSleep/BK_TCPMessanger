using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using TCPMessanger.Client.Commands;
using TCPMessanger.Client.Models;

namespace TCPMessanger.Client.ViewModels
{
    public class ClientViewModel: ViewModelBase
    {
        private string _ip;
        private int _port;
        private string _nickname;
        private string _chatTextArea;
        private string _message;
        StringBuilder _chatBuilder;
        private bool _isConnected;

        private TcpClientModel _tcpClientModel;

        public string IP
        {
            get
            {
                return _ip;
            }
            set
            {
                _ip = value;
                OnPropertyChanged(nameof(IP));
            }
        }

        public int Port
        {
            get
            {
                return _port;
            }
            set
            {
                _port = value;
                OnPropertyChanged(nameof(Port));
            }
        }

        public string Nickname
        {
            get
            {
                return _nickname;
            }
            set
            {
                _nickname = value;
                OnPropertyChanged(nameof(Nickname));
            }
        }

        public string ChatTextArea
        {
            get
            {
                return _chatTextArea;
            }
            set
            {
                _chatTextArea = value;
                OnPropertyChanged(nameof(ChatTextArea));
            }
        }

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        public bool IsBlocked
        {
            get
            {
                return !_isConnected;
            }
        }
        public bool IsConnected
        {
            get
            {
                return _isConnected;
            }
            set
            {
                _isConnected = value;
                OnPropertyChanged(nameof(IsConnected));
                OnPropertyChanged(nameof(IsBlocked));
            }
        }

        public ICommand ConnectCommand { get; }
        //public ICommand DisconnectCommand { get; }
        public ICommand MessageCommand { get; }

        public ClientViewModel(TcpClientModel tcpClientModel)
        {
            _ip = "127.0.0.1";
            _port = 5050;
            _nickname = "User";

            _chatBuilder = new StringBuilder();
            _tcpClientModel = tcpClientModel;
            ConnectCommand = new ConnectClientCommand(tcpClientModel, this);
            MessageCommand = new SendMessageCommand(tcpClientModel, this);

            _tcpClientModel.DataReceived += OnDataReceived;
        }

        private void OnDataReceived(object sender, TcpClientModel.DataReceivedEventArgs e)
        {
            _chatBuilder.Append(e.Data + "\n");
            ChatTextArea = _chatBuilder.ToString();
        }
    }
}
