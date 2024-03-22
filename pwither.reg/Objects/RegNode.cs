using pwither.reg.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace pwither.reg.Objects
{
    public class RegNode
    {
        public RegKeyDirectory Type { get; internal set; }
        public string Name { get; internal set; }
        public List<RegNodeValue> Values { get; internal set; }
        public List<RegNode> Nodes { get; internal set; }

        public bool Remove {  get; set; }

        public RegNode() 
        {
            Remove = false;
        }
        public RegNode(string name, List<RegNodeValue> values, List<RegNode> nodes, bool remove = false)
        {
            Name = name;
            Values = values;
            Nodes = nodes;
            Type = RegKeyDirectory.Sub;
            Remove = remove;
        }
    }
}
