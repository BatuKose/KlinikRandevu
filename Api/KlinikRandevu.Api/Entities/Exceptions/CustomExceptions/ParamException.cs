using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Exceptions.CustomExceptions
{
    public class ParamException:Exception
    {

        public ParamException(string message) : base(message) { }
    }
}
