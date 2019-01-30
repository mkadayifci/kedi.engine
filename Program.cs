using Microsoft.Owin.Hosting;
using System;

namespace kedi.engine
{
    class Program
    {


        const string url = "http://localhost:9000";
        static void Main(string[] args)
        {
            RegisterDI();
            using (WebApp.Start<Startup>(url))
            {
                Console.WriteLine("Server started at:" + url);
                Console.ReadLine();
            }
        }

        static void RegisterDI()
        {
            ContainerManager.Register();
        }
        //static void Main(string[] args)
        //{
        //    string dumpFile = @"C:\DumpAnalyze\Dumps\ScheduledService.DMP";
        //    var runTime = RuntimeBoss.CreateRuntime(dumpFile, null);
        //    var threadInfo = GetThreadInfo(runTime);

        //    foreach (var thread in threadInfo)
        //    {
        //        Console.WriteLine($"{thread.ManagedThreadId} - {thread.IsAlive} - {thread.CurrentException?.Message}");
        //        var indent = 0;
        //        foreach (var stack in thread.StackTrace)
        //        {
        //            Console.WriteLine($"{new String(' ', ++indent)} {stack.DisplayString}");
        //        }
        //    }

        //}

        //private static List<ThreadModel> GetThreadInfo(ClrRuntime runtime)
        //{
        //    List<ThreadModel> returnValue = new List<ThreadModel>();
        //    foreach (ClrThread thread in runtime.Threads)
        //    {
        //        var currentThreadModel = new ThreadModel()
        //        {
        //            IsAlive = thread.IsAlive,
        //            ManagedThreadId = thread.ManagedThreadId,
        //            OSThreadId = thread.OSThreadId
        //        };

        //        SetThreadModel(thread, currentThreadModel);
        //        SetStackTraceModel(thread, currentThreadModel);

        //        returnValue.Add(currentThreadModel);


        //        //Console.WriteLine("Thread {0:X}:", thread.OSThreadId);
        //        //Console.WriteLine("Stack: {0:X} - {1:X}", thread.StackBase, thread.StackLimit);


        //    }
        //    return returnValue;

        //}

        //private static void SetThreadModel(ClrThread thread, ThreadModel currentThreadModel)
        //{
        //    if (thread.CurrentException != null)
        //    {
        //        currentThreadModel.CurrentException = new Models.ExceptionModel()
        //        {
        //            Message = thread.CurrentException.Message
        //        };
        //    }
        //}

        //private static void SetStackTraceModel(ClrThread thread, ThreadModel currentThreadModel)
        //{
        //    var stackTraceModel = new List<StackFrameModel>();
        //    foreach (ClrStackFrame frame in thread.StackTrace)
        //    {
        //        stackTraceModel.Add(new StackFrameModel()
        //        {
        //            DisplayString = frame.DisplayString
        //        });
        //    }
        //    currentThreadModel.StackTrace = stackTraceModel;
        //}
    }
}
