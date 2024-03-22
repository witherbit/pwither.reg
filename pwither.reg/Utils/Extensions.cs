using Microsoft.Win32;
using pwither.reg.Enums;
using pwither.reg.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace pwither.reg.Utils
{
    public static class Extensions
    {
        public static byte[] ConvertToRegBinary(this string value)
        {
            byte[] result = new byte[value.Length / 3 + (value.EndsWith(" ") ? 0 : 1)];
            for (int i = 0; i < result.Length; ++i)
                result[i] = byte.Parse(
                                value.Substring(i * 3, 2),
                                System.Globalization.NumberStyles.HexNumber
                                );
            return result;
        }

        public static uint ConvertToRegDWORD(this string value)
        {
            return uint.Parse(value);
        }

        public static RegNode ToRegNode(this string path, List<RegNodeValue> values = null, RegRemoveType remove = RegRemoveType.NotRemove)
        {
            var spt = path.Split(new string[] { @"\", "/"}, StringSplitOptions.RemoveEmptyEntries);
            var nodes = new List<RegNode>();
            if (remove == RegRemoveType.RemoveAll) return new RegNode(spt[0], null, null, true);
            for (int i = 0; i < spt.Length; i++)
            {
                nodes.Add(new RegNode(spt[i], null, new List<RegNode>()));
            }
            if (nodes.Count < 1) return null;
            if (remove == RegRemoveType.RemoveLast)
                nodes.Last().Remove = true;
            nodes.Last().Values = values;
            for(int i = nodes.Count - 1; i > 0; i--)
            {
                nodes[i - 1].Nodes.Add(nodes[i]);
            }
            return nodes.First();
        }
        public static RegNode AddRegNode(this RegNode node, string path, List<RegNodeValue> values = null, RegRemoveType remove = RegRemoveType.NotRemove)
        {
            node.Nodes.Add(path.ToRegNode(values, remove));
            return node;
        }
        public static RegNode SetRegNode(this RegNode node, string path, int index, List<RegNodeValue> values = null, RegRemoveType remove = RegRemoveType.NotRemove)
        {
            node.Nodes[index] = path.ToRegNode(values, remove);
            return node;
        }
        public static RegNode AddChildRegNode(this RegNode node, string path, int childIndex, List<RegNodeValue> values = null, RegRemoveType remove = RegRemoveType.NotRemove)
        {
            node.Nodes[childIndex].Nodes.Add(path.ToRegNode(values, remove));
            return node;
        }
        public static RegNode SetChildRegNode(this RegNode node, string path, int childIndex, int index, List<RegNodeValue> values = null, RegRemoveType remove = RegRemoveType.NotRemove)
        {
            node.Nodes[childIndex].Nodes[index] = path.ToRegNode(values, remove);
            return node;
        }
        public static RegKey ToRegKey(this string path, List<RegValue> values = null)
        {
            var spt = path.Split(new string[] { @"\", "/" }, StringSplitOptions.RemoveEmptyEntries);
            var keys = new List<RegKey>();
            for (int i = 0; i < spt.Length; i++)
            {
                keys.Add(new RegKey { Keys = new List<RegKey>(), Name = spt[i],  Values = null});
            }
            if (keys.Count < 1) return null;
            keys.Last().Values = values;
            for (int i = keys.Count - 1; i > 0; i--)
            {
                keys[i - 1].Keys.Add(keys[i]);
            }
            return keys.First();
        }
        public static RegKey AddRegKey(this RegKey node, string path, List<RegValue> values = null)
        {
            node.Keys.Add(path.ToRegKey(values));
            return node;
        }
        public static RegKey SetRegKey(this RegKey node, string path, int index, List<RegValue> values = null)
        {
            node.Keys[index] = path.ToRegKey(values);
            return node;
        }
        public static RegKey AddChildRegKey(this RegKey node, string path, int childIndex, List<RegValue> values = null)
        {
            node.Keys[childIndex].Keys.Add(path.ToRegKey(values));
            return node;
        }
        public static RegKey SetChildRegKey(this RegKey node, string path, int childIndex, int index, List<RegValue> values = null)
        {
            node.Keys[childIndex].Keys[index] = path.ToRegKey(values);
            return node;
        }
    }
}
