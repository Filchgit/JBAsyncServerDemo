using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace JBAsyncTCPServer
{
    public class JBSocketServer
    {
        IPAddress myIP;
        int myPort;
        TcpListener myTCPListener;

        public async void StartListeningForIncomingConnection(IPAddress ipaddr = null, int port = 23000)
        // I need the async keyword in the method declare as I will be making an async call within it 
        {
            if (ipaddr == null)
            {
                ipaddr = IPAddress.Any;
            }
            if (port <= 0 || port >= 65535)
            {
                port = 23000;
            }
            myIP = ipaddr;
            myPort = port;

            System.Diagnostics.Debug.WriteLine(string.Format("IP Address: {0} - Port: {1)", myIP.ToString(), myPort.ToString()));

            myTCPListener = new TcpListener(myIP, port);
            myTCPListener.Start();

            var returnedByAccept = await myTCPListener.AcceptTcpClientAsync();

            System.Diagnostics.Debug.WriteLine("Client connected successfully." + returnedByAccept.ToString());
        }
    }
}
