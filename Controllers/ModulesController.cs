using kedi.engine.Services.Analyze;
using Microsoft.Diagnostics.Runtime;
using System.Collections.Generic;
using System.IO;
using System.Web.Http;
using static System.Diagnostics.DebuggableAttribute;

namespace kedi.engine.Controllers
{
    public class ModulesController : ApiController
    {
        IAnalyzeOrchestrator analyzeOrchestrator = ContainerManager.Container.Resolve<IAnalyzeOrchestrator>();


        [HttpGet]
        [Route("api/modules/{sessionId}")]
        public List<dynamic> Get([FromUri]string sessionId)
        {
            var returnValue = new List<dynamic>();

            ClrRuntime runtime = analyzeOrchestrator.GetRuntimeBySessionId(sessionId);


            foreach (var module in runtime.Modules)
            {
                dynamic currentModule =
                                         new
                                         {
                                             module.Name,
                                             Types = new List<dynamic>(),
                                             AppDomains = new List<dynamic>(),
                                             FullPath = module.FileName,
                                             FileName = Path.GetFileName(module.FileName),
                                             module.Pdb,
                                             module.AssemblyName,
                                             module.DebuggingMode,
                                             IsInDebugMode = IsInDebug(module.DebuggingMode)
                                         };

                foreach (var appDomain in module.AppDomains)
                {
                    currentModule.AppDomains.Add(new
                    {
                        appDomain.Address,
                        appDomain.Name
                    });
                }

                returnValue.Add(currentModule);
            }
            return returnValue;
        }

        private bool IsInDebug(DebuggingModes debugMode)
        {
            return (debugMode & DebuggingModes.Default) == DebuggingModes.Default;
        }
    }


}




//if (module.Name.Contains("Log2up"))
//{
//    var bytes1 = ReadMemory(runtime.DataTarget,65536- module.MetadataLength, module.ImageBase);
//    var bytes2 = ReadMemory(runtime.DataTarget, module.MetadataLength, module.MetadataAddress);
//    var bytes = new byte[bytes1.Length + bytes2.Length];
//    bytes2.CopyTo(bytes, 0);
//    bytes1.CopyTo(bytes, bytes2.Length);

//    var moduleFileName = System.IO.Path.GetFileName(module.FileName);
//    string outputFilePath = $"C:\\Users\\mkadayifci\\Desktop\\Module\\{moduleFileName}";
//    File.WriteAllBytes(outputFilePath, bytes);

//}


//public byte[] ReadMemory(DataTarget dataTarget, ulong imageSize, ulong imageBaseAddres)
//{
//    byte[] bytes = new byte[imageSize];
//    if (dataTarget.DataReader.ReadMemory(imageBaseAddres, bytes, bytes.Length, out var bytesRead))
//    {
//        return bytes;
//    }


//    return null;
//}