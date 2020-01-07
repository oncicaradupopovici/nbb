using System;

namespace NBB.Tenancy.Abstractions.Exceptions
{
    public class TenancyException : Exception
    {
        public TenancyException(string msg, Exception inner)
         : base(msg, inner)
        {
        }

        public TenancyException(string msg)
            : base(msg)
        {
        }
    }
}