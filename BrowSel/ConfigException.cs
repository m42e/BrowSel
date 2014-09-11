using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrowSel
{
    class ConfigException : Exception
    {
        public ConfigException(string p) : base(p)
        {
        }
    }
}
