using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPMessanger.Client.Models;
using TCPMessanger.Client.ViewModels;

namespace TCPMessanger.Client.Commands
{
    public class SendMessageCommand: AsyncCommandBase
    {
        private readonly TcpClientModel _tcpClientModel;
        private readonly ClientViewModel _clientViewModel;

        public SendMessageCommand(TcpClientModel tcpClientModel, ClientViewModel clientViewModel)
        {
            _tcpClientModel = tcpClientModel;
            _clientViewModel = clientViewModel;
            _clientViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        public override bool CanExecute(object parameter)
        {
            return !String.IsNullOrWhiteSpace(_clientViewModel.Message) &&
                   _tcpClientModel.Client.Connected &&
                base.CanExecute(parameter);
        }

        public override async Task ExecuteAsync(object parameter)
        {
            await _tcpClientModel.SendStringAsync($"{_clientViewModel.Nickname}: {_clientViewModel.Message}");
            _clientViewModel.Message = "";
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_clientViewModel.Message))
            {
                OnCanExecutedChanged();
            }
        }
    }
}
