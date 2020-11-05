using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        public bool KeepRunning { get; set; } 
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

            System.Diagnostics.Debug.WriteLine(string.Format($"IP Address: {ipaddr}  - Port: {port} "));
            // since we are using.System Diagnostics we can skip the System.Diagnostics bit really

            myTCPListener = new TcpListener(myIP, port);
            try
            {
                myTCPListener.Start();
                KeepRunning = true;
                while (KeepRunning)
                {
                    var returnedByAccept = await myTCPListener.AcceptTcpClientAsync();

                    Debug.WriteLine("Client connected successfully." + returnedByAccept.ToString());

                    TakeCareOfTCPClient(returnedByAccept);
                }
            }
            catch(Exception excp) 
            {
                Debug.WriteLine(excp.ToString());
            }
        }

        private async void TakeCareOfTCPClient(TcpClient paramClient)
        {
            NetworkStream stream = null;
            StreamReader reader = null;
            try 
            {
                stream = paramClient.GetStream();
                reader = new StreamReader(stream);
               
                char[] buff = new char[64];

                while(KeepRunning)
                {
                    Debug.WriteLine("Ready to read.");
                   int intReturned = await reader.ReadAsync(buff, 0, buff.Length);

                    Debug.WriteLine("Returned:" + intReturned);
                   
                    if (intReturned == 0)
                    {
                        Debug.WriteLine("Socket disconnected.");
                        //as a zero Intreturned means the stream has ended
                        break;
                    }
                }
            }
            catch (Exception excp)
            {
                Debug.WriteLine(excp.ToString());
            }
        }
    }
}
