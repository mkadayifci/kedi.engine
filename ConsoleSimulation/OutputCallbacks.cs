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
            Console.WriteLine(text);
            //switch (mask)
            //{
            //    case DEBUG_OUTPUT.ERROR:
            //        context.WriteError(text.TrimEnd('\n', '\r'));
            //        break;
            //    case DEBUG_OUTPUT.EXTENSION_WARNING:
            //    case DEBUG_OUTPUT.WARNING:
            //        context.WriteWarning(text.TrimEnd('\n', '\r'));
            //        break;
            //    case DEBUG_OUTPUT.SYMBOLS:
            //        context.WriteInfo(text.TrimEnd('\n', '\r'));
            //        break;
            //    default:
            //        context.WriteLine(text);
            //        break;
            //}

            var jj = GlobalHost.ConnectionManager.GetHubContext<SignalRHub>();
            jj.Clients.All.ReceiveMessage(text);
  

            return 0;
        }
    }
}
