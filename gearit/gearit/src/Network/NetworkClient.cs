using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using gearit.src.output;
using System.Threading;

namespace gearit.src.Network
{
    static class NetworkClient
    {
        public static int _port;
        public static string _host;
        private static NetClient s_client;
        public enum State
        {
            Connecting,
            Connected,
            Disconnected
        }
        private static State state = State.Disconnected;
        private static Mutex mutex = new Mutex();
        private static Thread clientThread;
        public static List<NetIncomingMessage> requests = new List<NetIncomingMessage>();

        public static State getState()
        {
            return (state);
        }

        private static void tryToConnect()
        {
            OutputManager.LogMessage("Trying to connect to: " + _host + ":" + _port);

            try
            {
                s_client.Start();
                NetOutgoingMessage hail = s_client.CreateMessage("Hello I want connection kthxbye");
                s_client.Connect(_host, _port, hail);
            }
            catch
            {
                OutputManager.LogError("CLIENT - Fail to connect to server");
            }
        }

        public static void Connect(string host, int port)
        {
            if (state == State.Connecting)
                return ;

            Disconnect();

            _port = port;
            _host = host;
            state = State.Connecting;

            NetPeerConfiguration config = new NetPeerConfiguration("gearit");
            s_client = new NetClient(config);
            s_client.RegisterReceivedCallback(new SendOrPostCallback(Receive));

            clientThread = new Thread(tryToConnect);
            clientThread.Start();
        }

        public static void Disconnect()
        {
            if (state == State.Connected)
            {
                s_client.Disconnect("Requested by user");
                s_client.Shutdown("Bye");
            }
        }

        public static void Receive(object peer)
        {
            NetIncomingMessage im = ((NetClient)peer).ReadMessage();
            OutputManager.LogMessage("Client received : " + im.MessageType);
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
                        state = State.Connected;
                    else
                        state = State.Disconnected;
                    OutputManager.LogError("CLIENT - Status Changed: " + status + " (" + im.ReadString() + ")");
                    break;
                case NetIncomingMessageType.Data:
                    manageRequest(im);
                    break;
                default:
                    OutputManager.LogError("CLIENT - Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes");
                    break;
            }
            s_client.Recycle(im);
        }

        public static void Send(string text)
		{
            if (state == State.Connected)
            {
                NetOutgoingMessage om = s_client.CreateMessage();
                om.Write(text);
                s_client.SendMessage(om, s_client.Connections, NetDeliveryMethod.ReliableOrdered, 0);
            }
        }

        public static void manageRequest(NetIncomingMessage msg)
        {
            OutputManager.LogMessage("Client received new request");
            requests.Add(msg);
        }
    }
}
