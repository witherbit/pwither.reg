using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;

namespace pwither.reg.Objects
{
    public class RegValue
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public RegistryValueKind Kind { get; set; }

        public RegValue(string name, object value, RegistryValueKind kind = RegistryValueKind.String) 
        {
            Name = name;
            Value = value;
            Kind = kind;
        }
    }
}
