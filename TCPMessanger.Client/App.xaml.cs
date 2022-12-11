using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TCPMessanger.Client.Models;
using TCPMessanger.Client.ViewModels;

namespace TCPMessanger.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly TcpClientModel _clientModel;

        public App()
        {
            _clientModel = new TcpClientModel();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(new ClientViewModel(_clientModel))
            };

            MainWindow.Show();

            base.OnStartup(e);
        }
    }
}
