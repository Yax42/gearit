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
            NetPeerConfiguration config = new NetPeerConfiguration("gear it");
            config.AutoFlushSendQueue = false;
            s_client = new NetClient(config);

            s_client.RegisterReceivedCallback(new SendOrPostCallback(Receive));
        }

        [STAThread]
        public void Start()
        {
            s_client.Start();
            NetOutgoingMessage hail = s_client.CreateMessage("(Lidgren)Connected to the server");
            s_client.Connect(_host, _port, hail);
        }

        public void Stop()
        {
            s_client.Disconnect("Requested by user");
            s_client.Shutdown("Bye");
        }

        public static void clientMainLoop(object peer)
        {
            
        }

        public static void Receive(object peer)
        {
            NetIncomingMessage im;
            while ((im = s_client.ReadMessage()) != null)
            {
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
                            OutputManager.LogError("(Lidgren)Disconnected");
                            _connected = false;
                        }

                        /*if (status == NetConnectionStatus.Disconnected)
                            "Connect";*/

                        string reason = im.ReadString();

                        break;
                    case NetIncomingMessageType.Data:
                        string msg = im.ReadString();
                        OutputManager.LogMessage("(Lidgren)msg:" + msg);
                        break;
                    default:
                        //Output("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes");
                        OutputManager.LogError("(Lidgren)Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes");
                        break;
                }
                s_client.Recycle(im);
            }
        }

        public void Send(string text)
		{
			NetOutgoingMessage om = s_client.CreateMessage(text);
			s_client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
