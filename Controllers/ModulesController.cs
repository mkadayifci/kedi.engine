using kedi.engine.Services.Analyze;
using Microsoft.Diagnostics.Runtime;
using System.Collections.Generic;
using System.IO;
using System.Web.Http;

namespace kedi.engine.Controllers
{
    public class ModulesController : ApiController
    {
        IAnalyzeOrchestrator analyzeOrchestrator = ContainerManager.Container.Resolve<IAnalyzeOrchestrator>();


        public byte[] ReadMemory(DataTarget dataTarget, ulong imageSize, ulong imageBaseAddres)
        {
            byte[] bytes = new byte[imageSize];
            if (dataTarget.DataReader.ReadMemory(imageBaseAddres, bytes, bytes.Length, out var bytesRead))
            {
                return bytes;
            }


            return null;
        }

        //private byte[] ReadModuleImageFromRuntime(ClrRuntime runtime,ulong imageSize,ulong imageBaseAddress)
        //{
        //    byte[] content = new byte[imageSize];
        //    byte[] temporaryContent = new byte[imageSize];
        //    uint currentOffset = 0;

        //    while (currentOffset < imageSize)
        //    {
        //        var readResult= runtime.ReadMemory(imageBaseAddress +  currentOffset , temporaryContent, temporaryContent.Length - (int)currentOffset, out int bytesRead);
        //        temporaryContent.CopyTo(content, currentOffset);
        //        currentOffset += (uint)bytesRead;
        //    }


        //    return content;
        //}

        [HttpGet]
        [Route("api/modules/{sessionId}")]
        public List<dynamic> Get([FromUri]string sessionId)
        {
            var returnValue = new List<dynamic>();

            ClrRuntime runtime = analyzeOrchestrator.GetRuntimeBySessionId(sessionId);

            //foreach (var module in runtime.DataTarget.EnumerateModules())


            //{


            //    dynamic currentModule =
            //   new
            //   {

            //       AppDomains = new List<dynamic>(),
            //       module.FileName,


            //   };


            //    returnValue.Add(currentModule);
            //    //var file = module.GetPEFile();
            //    //if (file != null)
            //    //{

            //    //}




            //    //byte[] buffer = ReadMemory(runtime.DataTarget, module.FileSize, module.ImageBase);
            //    //var moduleFileName = System.IO.Path.GetFileName(module.FileName);
            //    //System.IO.File.WriteAllBytes($"C:\\Users\\mkadayifci\\Desktop\\Module\\{moduleFileName}", buffer);

            //}

            foreach (var module in runtime.Modules)
            {
                dynamic currentModule =
                 new
                 {
                     module.Name,
                     AppDomains = new List<dynamic>(),
                     FullPath=module.FileName,
                     FileName= System.IO.Path.GetFileName(module.FileName),
                     module.Pdb,
                     module.AssemblyName,
                     module.DebuggingMode
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


                
                //var bytes1 = ReadMemory(runtime.DataTarget, 960, module.ImageBase);
                //var bytes2 = ReadMemory(runtime.DataTarget, module.MetadataLength, module.MetadataAddress);
                //var bytes = new byte[bytes1.Length + bytes2.Length];
                //bytes2.CopyTo(bytes, 0);
                //bytes1.CopyTo(bytes, bytes2.Length);

                //var moduleFileName = System.IO.Path.GetFileName(module.FileName);
                //string outputFilePath = $"C:\\Users\\mkadayifci\\Desktop\\Module\\{moduleFileName}";
                //File.WriteAllBytes(outputFilePath, bytes);
            }
            return returnValue;
        }
    }
}

