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

        List<TcpClient> myTcpClients;

        public bool KeepRunning { get; set; }

        public JBSocketServer()
        {
            myTcpClients = new List<TcpClient>();
        }
        
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

                    myTcpClients.Add(returnedByAccept);
                    //so if a new Tcp Client connects we add them to our Tcp Client List. . . . 

                    Debug.WriteLine($"Client connected successfully, count {0} - {1}"
                        ,myTcpClients.Count, returnedByAccept.Client.RemoteEndPoint);

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
                    Debug.WriteLine("**Ready to read.**");
                   int intReturned = await reader.ReadAsync(buff, 0, buff.Length);

                    Debug.WriteLine("Returned:" + intReturned);
                   
                    if (intReturned == 0)
                    {
                        RemoveTcpClient(paramClient);

                        Debug.WriteLine("Socket disconnected.");
                        //as a zero Intreturned means the stream has ended
                        break;
                    }
                    string receivedText = new string(buff);
                    
                    Debug.WriteLine("Received: " + receivedText);
                    // need to clear the buff array after writing/using each time otherwise it will be garbled

                    Array.Clear(buff, 0, buff.Length);
                }
            }
            catch (Exception excp)
            {
                RemoveTcpClient(paramClient);

                Debug.WriteLine(excp.ToString());
            }
        }

        private void RemoveTcpClient(TcpClient paramClient)
        {
          if(myTcpClients.Contains(paramClient))
            {
                myTcpClients.Remove(paramClient);
                Debug.WriteLine($"client removed, count {0} ", myTcpClients.Count);
            }
        }
        public async void SendToAll (string allMessage)
        {
            if (string.IsNullOrEmpty(allMessage))
            {
                return;
            }
            try 
            {
                byte[] buffMessage = Encoding.ASCII.GetBytes(allMessage);
                foreach (TcpClient thisTcpClient in myTcpClients)
                {
                    thisTcpClient.GetStream().WriteAsync(buffMessage, 0, buffMessage.Length);
                        //so this gets the networkstream associated with this TCP CLient and writes to it async
                }
            }
            catch (Exception excp)
            {
                Debug.WriteLine(excp.ToString());
            }
        }
    
    }
}
