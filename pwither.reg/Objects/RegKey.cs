using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;

namespace pwither.reg.Objects
{
    public class RegKey
    {
        public string Name { get; set; }
        public List<RegValue> Values { get; set; }
        public List<RegKey> Keys { get; set; }
    }
}
