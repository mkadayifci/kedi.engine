using kedi.engine.Services.Analyze;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kedi.engine.Services.Sessions
{
    public class Session
    {

        
        public string Identifier { get; set; }
        public string SessionId { get; set; }
        public string SourceName { get; set; }
        
        public Session()
        {
            

        }
    }
}
