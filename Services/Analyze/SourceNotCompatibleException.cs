using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kedi.engine.Services.Analyze
{
    public class SourceNotCompatibleException : Exception
    {
        public SourceNotCompatibleException(string message) : base(message)
        {

        }
    }
}
