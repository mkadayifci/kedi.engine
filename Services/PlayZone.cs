using kedi.engine.MemoryRepresentation;
using kedi.engine.Services.Analyze;
using System.Collections.Generic;
using System.Linq;

namespace kedi.engine.Services
{
    public class PlayZone
    {
        const int ROW_COUNT = 500;
        const int MAX_DEPTH = 25;

        IAnalyzeOrchestrator analyzeOrchestrator = ContainerManager.Container.Resolve<IAnalyzeOrchestrator>();

        public List<string> GetTypes(string sessionId)
        {
            var memoryMap = analyzeOrchestrator.GetMemoryMapBySessionId(sessionId);

            var typeNames = memoryMap.Dictionary.Values
                .Where(item => item.TypeName != null)
                .Select(item => item.TypeName)
                .Distinct()
                .OrderBy(item => item)
                .ToList();

            return typeNames;
        }

        public dynamic GetResults(string sessionId,  string[] types, string queryValue = "")
        {

            var memoryMap = analyzeOrchestrator.GetMemoryMapBySessionId(sessionId);
            List<dynamic> returnValue = new List<dynamic>();

            foreach (var item in memoryMap.Dictionary)
            {
                if (types.Length == 0 || types.Contains(item.Value.TypeName)) //We found one of the requested objects
                {

                    var searchResult = IsObjectContainsValue(memoryMap, item.Value.Address, queryValue);
                    if (searchResult.IsSuccess)
                    {

                        returnValue.Add(
                            new
                            {
                                FoundAt = searchResult,
                                item.Value
                            });

                        if (PlayZone.ROW_COUNT == returnValue.Count)
                        {
                            break;
                        }
                    }
                }
            }
            return returnValue;
        }

        /// <summary>
        /// Checks object for the given value. 
        /// If object already a System.String object searches Value of the object 
        /// else search for members that is System.String
        /// </summary>
        /// <param name="value"></param>
        private SearchResult IsObjectContainsValue(MemoryMap memoryMap, ulong objectAddress, string searchValue , int depth=1)
        {
            searchValue = searchValue ?? "";

            MemoryObject memoryObject = memoryMap.Dictionary[objectAddress];

            if (memoryObject == null ||
                memoryObject.Value == null ||
                depth == PlayZone.MAX_DEPTH)
                return new SearchResult() { IsSuccess = false };

            if (memoryObject.Value.ToString().Contains(searchValue))
            {
                return new SearchResult()
                {
                    IsSuccess = true,
                    FoundAddress = objectAddress,
                    Path = memoryObject.TypeName,
                    FullContent = memoryObject.Value.ToString()
                };
            }

            foreach (var item in memoryObject.ReferencedObjects)
            {
                var innerSearchResult = this.IsObjectContainsValue(memoryMap, item.Address, searchValue,depth+1);

                if (innerSearchResult.IsSuccess)
                {
                    return new SearchResult()
                    {
                        IsSuccess = true,
                        FoundAddress = innerSearchResult.FoundAddress,
                        Path = $"{memoryObject.TypeName} -> {innerSearchResult.Path}",
                        FullContent = innerSearchResult.FullContent
                    };
                }
                return innerSearchResult;
            }

            return new SearchResult() { IsSuccess = false };

        }

        public class SearchResult
        {
            public bool IsSuccess { get; set; }
            public string Path { get; set; }
            public ulong FoundAddress { get; set; }
            public string FullContent { get; set; }


        }

    }


}
