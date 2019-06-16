using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Exceptions
{
    public abstract class RequestException : Exception
    {
        public RequestException() { }
        public RequestException(string message) : base(message) { }
    }
}
