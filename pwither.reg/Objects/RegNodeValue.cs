using Microsoft.Win32;
using pwither.reg.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace pwither.reg.Objects
{
    public class RegNodeValue
    {
        public string Name { get; internal set; }
        public object Value { get; internal set; }
        public RegistryValueKind Kind { get; internal set; }

        public bool Remove { get; set; }

        public RegNodeValue()
        {
            Remove = false;
        }

        public RegNodeValue(string name, RegistryValueKind kind = RegistryValueKind.String, bool remove = false)
        {
            Name = name;
            Kind = kind;
            Remove = remove;
        }

        public RegNodeValue(string name, bool remove)
        {
            Name = name;
            Remove = remove;
        }

        public byte[] GetBinary()
        {
            return Value as byte[];
        }

        public uint GetDWORD()
        {
            return (uint)Value;
        }

        public string GetString()
        {
            return Value.ToString();
        }
    }
}
