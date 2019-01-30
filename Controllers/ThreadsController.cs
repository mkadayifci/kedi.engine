﻿using kedi.engine.Services.Analyze;
using Microsoft.Diagnostics.Runtime;
using Microsoft.Diagnostics.Runtime.Interop;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.InteropServices;
using System.Web.Http;

namespace kedi.engine.Controllers
{
    public class ThreadsController : ApiController
    {
        IAnalyzeOrchestrator analyzeOrchestrator = ContainerManager.Container.Resolve<IAnalyzeOrchestrator>();

        [HttpGet]
        [Route("api/threads/{sessionId}")]
        public dynamic GetThreads([FromUri]string sessionId)
        {
            List<dynamic> threads = new List<dynamic>();
            ClrRuntime clrRuntime = analyzeOrchestrator.GetRuntimeBySessionId(sessionId);
 
            foreach (ClrThread thread in clrRuntime.Threads)
            {
                dynamic threadObject = new ExpandoObject();

                threadObject.OSThreadId = thread.OSThreadId;
                threadObject.ManagedThreadId = thread.ManagedThreadId;
                threadObject.IsFinalizer = thread.IsFinalizer;
                threadObject.IsGC = thread.IsGC;
                threadObject.Address = thread.Address;
                threadObject.GcMode = thread.GcMode;
                threadObject.IsAlive = thread.IsAlive;
                threadObject.IsBackground = thread.IsBackground;                
                threadObject.CurrentExceptionAddress = thread.CurrentException?.Address;
                threadObject.CurrentExceptionType = thread.CurrentException?.Type.Name;
                threadObject.StackTrace = new List<dynamic>();
                threadObject.StackObjects = new List<dynamic>();

                SetBasicThreadInfo(clrRuntime, threadObject, thread.Teb);

                foreach (ClrStackFrame frame in thread.StackTrace)
                {
                    threadObject.StackTrace.Add(new
                    {
                        frame.DisplayString,
                        frame.ModuleName,
                    });
                }

                foreach (var stackObj in thread.EnumerateStackObjects())
                {
                    var type = clrRuntime.Heap.GetObjectType(stackObj.Object);
                    threadObject.StackObjects.Add(new
                    {
                        TypeName = stackObj.Type.Name,
                        Address = stackObj.Address

                    });
                }
                threads.Add(threadObject);


            }

            return threads;
        }


        private void SetBasicThreadInfo(ClrRuntime runtime,dynamic threadObject, ulong threadTEB)
        {
            dynamic returnValue = new ExpandoObject();


            var debugClient = runtime.DataTarget.DebuggerInterface;

            ((IDebugSystemObjects)debugClient).GetThreadIdByTeb(threadTEB, out uint engineId);

            int size = Marshal.SizeOf(typeof(DEBUG_THREAD_BASIC_INFORMATION));
            var buffer = new byte[size];
            if (((IDebugAdvanced2)debugClient).
                GetSystemObjectInformation(DEBUG_SYSOBJINFO.THREAD_BASIC_INFORMATION, 0, engineId, buffer, buffer.Length, out size) >= 0)
            {
                GCHandle gcHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                try
                {
                    var basicInfo = (DEBUG_THREAD_BASIC_INFORMATION)Marshal.PtrToStructure(gcHandle.AddrOfPinnedObject(), typeof(DEBUG_THREAD_BASIC_INFORMATION));

                    if ((basicInfo.Valid & DEBUG_TBINFO.TIMES) != 0)
                    {
                        threadObject.CreationTime = DateTime.FromFileTimeUtc((long)basicInfo.CreateTime);
                        threadObject.ExitTime = basicInfo.ExitTime == (ulong)0 ? DateTime.MinValue : DateTime.FromFileTimeUtc((long)basicInfo.ExitTime);
                        threadObject.KernelTime = new TimeSpan((long)basicInfo.KernelTime);
                        threadObject.UserTime = new TimeSpan((long)basicInfo.UserTime);
                        threadObject.KernelTimeMiliseconds = Math.Floor( threadObject.KernelTime.TotalMilliseconds);
                        threadObject.UserTimeMiliseconds = Math.Floor(threadObject.UserTime.TotalMilliseconds);
                    }
                    if ((basicInfo.Valid & DEBUG_TBINFO.PRIORITY) != 0)
                    {
                        threadObject.Priority = basicInfo.Priority;
                    }
                    if ((basicInfo.Valid & DEBUG_TBINFO.PRIORITY_CLASS) != 0)
                    {
                        threadObject.PriorityClass = basicInfo.PriorityClass;
                    }
                    if ((basicInfo.Valid & DEBUG_TBINFO.EXIT_STATUS) != 0)
                    {
                        threadObject.ExitStatus = basicInfo.ExitStatus;
                    }
                    if ((basicInfo.Valid & DEBUG_TBINFO.START_OFFSET) != 0)
                    {
                        threadObject.StartOffset = basicInfo.StartOffset;
                    }
                }
                finally
                {
                    gcHandle.Free();
                }
            }
        }

    }
}