using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Capstone.Project.Data.Helper
{
    public class AppException : Exception
    {
        public AppException() : base() { }

        public AppException(string message) : base(message) { }

        public AppException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {

        }
    }
}
