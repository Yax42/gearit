using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using gearit.src.output;

using Lidgren.Network;

namespace gearit.src.server
{
    class InGameClient
    {
        public int _port;
        public string _host;
        private static NetClient s_client;
        private static bool _connected;

        public InGameClient(int port, string host)
        {
            _port = port;
            _host = host;
            OutputManager.LogMessage("host:" + _host + " port:" + _port);
            NetPeerConfiguration config = new NetPeerConfiguration("gearit");
            s_client = new NetClient(config);

            s_client.RegisterReceivedCallback(new SendOrPostCallback(Receive));
        }

        public void Start()
        {
            try
            {
                s_client.Start();
                NetOutgoingMessage hail = s_client.CreateMessage("CLIENT - Connected to the server");
                s_client.Connect(_host, _port, hail);
            }
            catch
            {
                OutputManager.LogError("CLIENT - Fail to connect to server");
            }
        }

        public void Stop()
        {
            if (_connected)
            {
                s_client.Disconnect("Requested by user");
                s_client.Shutdown("Bye");
            }
        }

        public static void clientMainLoop(object peer)
        {
            
        }

        public static void Receive(object peer)
        {
            NetIncomingMessage im;
            while ((im = s_client.ReadMessage()) != null)
            {
                OutputManager.LogMessage("Client received : " + im.MessageType);
                // handle incoming message
                switch (im.MessageType)
                {
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
                        string text = im.ReadString();
                        OutputManager.LogError(text);
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();

                        if (status == NetConnectionStatus.Connected)
                            _connected = true;
                        else
                        {
                            OutputManager.LogError("CLIENT - Disconnected");
                            _connected = false;
                        }

                        if (status == NetConnectionStatus.Disconnected)
                            _connected = false;

                        string reason = im.ReadString();

                        break;
                    case NetIncomingMessageType.Data:
                        string msg = im.ReadString();
                        OutputManager.LogMessage("CLIENT -  msg: " + msg);
                        Send("Hello i'm client");
                        break;
                    default:
                        //Output("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes");
                        OutputManager.LogError("CLIENT - Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes");
                        break;
                }
                s_client.Recycle(im);
            }
        }

        public static void Send(string text)
		{
            if (_connected)
            {
                NetOutgoingMessage om = s_client.CreateMessage();
                om.Write(text);
                s_client.SendMessage(om, s_client.Connections, NetDeliveryMethod.ReliableOrdered, 0);
            }
        }
    }
}
