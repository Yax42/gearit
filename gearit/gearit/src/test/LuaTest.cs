using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuaInterface;
using gearit.xna;

using gearit.src.robot;
using gearit.src.robot.api;

namespace gearit.src.utility
{


    class LuaTest
    {
        private Robot _robot;
        private List<Api> _api;
        private Input _input;
        private String _name;
        private MyGame _game;
        public Lua _luaInterpret;

        public void saySomething(string something)
        {
            Console.WriteLine(something);
        }

        public LuaTest(Camera2D camera, Robot robot, string name)
        {
            _input = new Input(camera);
            _robot = robot;
            _name = name;
            _api = robot.getApi();
            _luaInterpret = new Lua();
            _luaInterpret.RegisterFunction("getRobot", this, this.GetType().GetMethod("getRobot"));
            _luaInterpret.RegisterFunction("getKeysAction", _input, _input.GetType().GetMethod("getKeysAction"));
            for (int i = _api.Count - 1; i >= 0; i--)
                _luaInterpret[_api[i].name()] = _api[i];
        }

        public void getRobot()
        {
            Console.WriteLine(" {0} = {1}", _api[0].name(), _api[0].motorForce);
            Console.WriteLine(" {0} = {1}", _api[1].name(), _api[1].motorForce);
        }

        public void execFile()
        {
            _luaInterpret.DoFile(@"scripts/" + _name + ".lua");
        }
    }
}
