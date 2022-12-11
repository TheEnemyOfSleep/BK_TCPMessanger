using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TCPMessanger.Client.Models;
using TCPMessanger.Client.ViewModels;

namespace TCPMessanger.Client.Commands
{
    public class ConnectClientCommand: AsyncCommandBase
    {
        private readonly TcpClientModel _tcpClientModel;
        private readonly ClientViewModel _clientViewModel;

        public ConnectClientCommand(TcpClientModel tcpClientModel, ClientViewModel clientViewModel)
        {
            _tcpClientModel = tcpClientModel;
            _clientViewModel = clientViewModel;
            _clientViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        public override bool CanExecute(object parameter)
        {
            return !string.IsNullOrEmpty(_clientViewModel.IP) &&
                   _clientViewModel.Port > 0 &&
                   !string.IsNullOrEmpty(_clientViewModel.Nickname) &&
                   !_tcpClientModel.Client.Connected &&
                   base.CanExecute(parameter);
        }

        public override async Task ExecuteAsync(object parameter)
        {
            await _tcpClientModel.ConnectClient(IPAddress.Parse(_clientViewModel.IP), _clientViewModel.Port);
            await _tcpClientModel.SendStringAsync($"Login: {_clientViewModel.Nickname}");
            _clientViewModel.IsConnected = true;
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_clientViewModel.IP) ||
                e.PropertyName == nameof(_clientViewModel.Port) ||
                e.PropertyName == nameof(_clientViewModel.Nickname))
            {
                OnCanExecutedChanged();
            }
        }
    }
}
