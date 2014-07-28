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

        public string Serialize()
        {
            return String.Concat(id, String.Join(String.Empty, data));
        }
    }

    class RequestBuilder
    {
        public const int SAY_READY = 20;
        public const int ROBOT_SELECT = 21;
        public const int ROBOT_DL = 22;
        public const int DLR_STATUS = 23;
        public const int MAP_SELECT = 24;
        public const int MAP_CHECK = 25;
        public const int MAP_OK = 26;
        public const int MAP_KO = 27;
        public const int MAP_DL = 28;
        public const int MAP_DL_OK = 29;
        public const int MAP_DL_KO = 30;
        public const int START_GAME = 31;
        public const int END_GAME = 32;
        public const int OK = 4;
        public const int KO = 5;

    }
}
