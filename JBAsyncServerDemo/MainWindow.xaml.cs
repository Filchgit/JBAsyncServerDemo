using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using JBAsyncTCPServer;

namespace JBAsyncServerDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        JBSocketServer myServer;
        public MainWindow()
        {
            InitializeComponent();
            myServer = new JBSocketServer();
            myServer.RaiseClientConnectedEvent += HandleClientConnected;
            myServer.RaiseTextReceivedEvent += HandleTextReceived;
        }

        private void btnAcceptIncConn_Click(object sender, RoutedEventArgs e)
        {
            myServer.StartListeningForIncomingConnection();
        }

        private void btnSendAllClients_Click(object sender, RoutedEventArgs e)
        {
            myServer.SendToAll(txtMessageBox.Text.Trim());
        }

        private void btnStopServer_Click(object sender, RoutedEventArgs e)
        {
            myServer.StopServer();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            myServer.StopServer();
        }

        void HandleClientConnected(object sender, ClientConnectedEventArgs ccea)
        {
            txtConsole.AppendText($"{DateTime.Now} - New Tcp client connected : {ccea.NewClient.ToString()}  ");
            txtConsole.AppendText(Environment.NewLine);
        }
        void HandleTextReceived(object sender, TextReceivedEventArgs trea )
        {
            txtConsole.AppendText($"{DateTime.Now} - Received from {trea.ClientThatSentText} : {trea.TextReceived}");
            txtConsole.AppendText(Environment.NewLine);
        }
    }
}
