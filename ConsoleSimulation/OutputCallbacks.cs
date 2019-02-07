using kedi.engine.ConsoleSimulation;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Diagnostics.Runtime.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kedi.engine.ConsoleSimulation{
    public class OutputCallbacks : IDebugOutputCallbacksWide
    {
        public OutputCallbacks()
        {
        }

        public int Output(DEBUG_OUTPUT mask, string text)
        {
            switch (mask)
            {
                case DEBUG_OUTPUT.ERROR:
                    Console.Write(text.TrimEnd('\n', '\r'));
                    break;
                case DEBUG_OUTPUT.EXTENSION_WARNING:
                case DEBUG_OUTPUT.WARNING:
                    Console.Write(text.TrimEnd('\n', '\r'));
                    break;
                case DEBUG_OUTPUT.SYMBOLS:
                    Console.Write(text.TrimEnd('\n', '\r'));
                    break;
                default:
                    Console.Write(text );
                    break;
            }

            var jj = GlobalHost.ConnectionManager.GetHubContext<SignalRHub>();
            jj.Clients.All.ReceiveMessage(text);
  

            return 0;
        }
    }
}
