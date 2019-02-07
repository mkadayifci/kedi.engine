using Microsoft.Diagnostics.Runtime;
using Microsoft.Diagnostics.Runtime.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kedi.engine
{
    public class RuntimeBoss
    {

        public static ClrRuntime CreateRuntime(string dump, string dac)
        {
            DataTarget dataTarget = DataTarget.LoadCrashDump(dump);
            
            bool isTarget64Bit = dataTarget.PointerSize == 8;
            if (Environment.Is64BitProcess != isTarget64Bit)
                throw new Exception(string.Format("Architecture mismatch:  Process is {0} but target is {1}", Environment.Is64BitProcess ? "64 bit" : "32 bit", isTarget64Bit ? "64 bit" : "32 bit"));

            ClrInfo version = dataTarget.ClrVersions[0];

            dac = dataTarget.SymbolLocator.FindBinary(version.DacInfo);


            var debuggerControl = (IDebugControl5)dataTarget.DebuggerInterface;
            debuggerControl.AddExtension(@"C:\DumpAnalyze\x64\SOS.dll", 0, out ulong handle);


            if (dac == null || !File.Exists(dac))
                throw new FileNotFoundException("Could not find the specified dac.", dac);

            return version.CreateRuntime(dac);

        }

    }
}
