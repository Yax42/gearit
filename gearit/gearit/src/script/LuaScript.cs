using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuaInterface;
using gearit.xna;
using System.Threading;

using gearit.src.robot;
using gearit.src.editor.api;

namespace gearit.src.utility
{

    class LuaScript : Lua
    {
        private String _name;
        private List<PieceApi> _api;
        private InputApi _inputApi;
        private Thread _thread;

        public void saySomething(string something)
        {
            Console.WriteLine(something);
        }

        public LuaScript(List<PieceApi> api, string name)
        {
            _name = name;
	    _api = api;
            _inputApi = new InputApi();
	    _thread = new Thread(new ThreadStart(exec));
            for (int i = 0; i < api.Count; i++)
                this[api[i].name()] = api[i];
            this["Input"] = _inputApi;
            //RegisterFunction("getKeysAction", _input, _input.GetType().GetMethod("getKeysAction"));
            run();
        }
	
	private void run()
	{
	    _thread.Start();
	}

        private void exec()
        {
            DoFile(@"data/script/" + _name + ".lua");
        }

        public void stop()
        {
            _thread.Abort();
            base.Close();
        }
    }
}
