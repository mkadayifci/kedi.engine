using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kedi.engine.Services.Sessions
{
    public class SessionNotFoundException:Exception
    {
        public SessionNotFoundException(string message):base(message)
        {

        }
    }
}
