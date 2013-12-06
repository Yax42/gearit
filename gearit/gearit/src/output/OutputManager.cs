using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gearit.src.output
{
    class OutputManager
    {
        // STATIC method
        private static OutputManager _instance;
        private IOutputMessageManager _output_message_manager;

        public static OutputManager GetInstance()
        {
            if (_instance == null)
                _instance = new OutputManager();
            return _instance;
        }
        public static void SetInstance(OutputManager om)
        {
            _instance = om;
        }
        // END

        public IOutputMessageManager GetOutputMessageManager()
        {
            if (_instance == null)
                throw new System.MethodAccessException("_output_message_manager was not set");
            return _output_message_manager;
        }
        public void SetOutputMessageManager(IOutputMessageManager value)
        {
            _output_message_manager = value;
        }
    }
}
