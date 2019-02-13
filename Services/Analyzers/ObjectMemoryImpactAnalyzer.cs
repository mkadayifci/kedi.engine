﻿using kedi.engine.Services.Analyze;
using Microsoft.Diagnostics.Runtime;
using System.Collections.Generic;
using System.Linq;

namespace kedi.engine.Services.Analyzers
{
    public class ObjectMemoryImpactAnalyzer
    {
        IAnalyzeOrchestrator analyzeOrchestrator = ContainerManager.Container.Resolve<IAnalyzeOrchestrator>();

        public dynamic Analyze(string sessionId)
        {
            List<dynamic> returnValue = new List<dynamic>();
            ClrRuntime runtime = analyzeOrchestrator.GetRuntimeBySessionId(sessionId);

            foreach (var objPointer in runtime.Heap.EnumerateObjectAddresses())
            {
                var type = runtime.Heap.GetObjectType(objPointer);

                if (type.IsException)
                {
                    var exceptionObject = runtime.Heap.GetExceptionObject(objPointer);
                    var exceptionDetail = (new
                    {

                        ObjectPointer = exceptionObject.Address,
                        TypeName = exceptionObject.Type.Name,
                        exceptionObject.Message,
                        exceptionObject.HResult,
                        Method = exceptionObject.StackTrace?.LastOrDefault<ClrStackFrame>()?.DisplayString,
                        StackTrace = new List<dynamic>()
                    });



                    foreach (ClrStackFrame frame in exceptionObject.StackTrace)
                    {
                        exceptionDetail.StackTrace.Add(new
                        {
                            frame.DisplayString,
                            frame.ModuleName,
                        });
                    }
                    returnValue.Add(exceptionDetail);
                }
            }
            return returnValue;
        }
    }
}
