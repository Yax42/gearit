using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lidgren.Network;

namespace gearit.src.server
{
    struct Request
    {
        public int id;
        public List<string> data;

        public string ToString()
        {
            if (data == null)
            {
                return id.ToString();
            }
            else
            {
                return String.Concat(id, String.Join(String.Empty, data));
            }
        }
    }

    class RequestBuilder
    {
        public const int REQ_SAY_READY = 20;
        public const int REQ_ROBOT_SELECT = 21;
        public const int REQ_ROBOT_DL = 22;
        public const int REQ_DLR_STATUS = 23;
        public const int REQ_MAP_SELECT = 24;
        public const int REQ_MAP_CHECK = 25;
        public const int REQ_MAP_OK = 26;
        public const int REQ_MAP_KO = 27;
        public const int REQ_MAP_DL = 28;
        public const int REQ_MAP_DL_OK = 29;
        public const int REQ_MAP_DL_KO = 30;
        public const int REQ_START_GAME = 31;
        public const int REQ_END_GAME = 32;
        public const int REQ_OK = 4;
        public const int REQ_KO = 5;

        public static Request Build(int id, List<string> data)
        {
            Request req = new Request();
            req.id = id;
            req.data = data;

            return (req);
        }

        public static void Send(NetClient sender, Request req)
        {
            NetOutgoingMessage outmsg = sender.CreateMessage();

            outmsg.Write(req.ToString());
            sender.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
