using Microsoft.Win32;
using pwither.reg.Enums;
using pwither.reg.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace pwither.reg
{
    public sealed class Reg
    {
        public RegistryKey HKEY_CLASSES_ROOT { get; private set; }
        public RegistryKey HKEY_CURRENT_USER { get; private set; }
        public RegistryKey HKEY_LOCAL_MACHINE { get; private set; }
        public RegistryKey HKEY_USERS { get; private set; }
        public RegistryKey HKEY_CURRENT_CONFIG { get; private set; }

        public Reg()
        {
            HKEY_CLASSES_ROOT = Registry.ClassesRoot;
            HKEY_CURRENT_USER = Registry.CurrentUser;
            HKEY_LOCAL_MACHINE = Registry.LocalMachine;
            HKEY_USERS = Registry.Users;
            HKEY_CURRENT_CONFIG = Registry.CurrentConfig;
        }

        public RegNode Write(RegKey key, RegKeyDirectory keyDir)
        {
            var node = new RegNode
            {
                Name = key.Name,
                Type = keyDir
            };
            RegistryKey mKey = HKEY_CLASSES_ROOT;
            switch (keyDir)
            {
                case RegKeyDirectory.HKEY_CURRENT_USER: mKey = HKEY_CURRENT_USER; break;
                case RegKeyDirectory.HKEY_LOCAL_MACHINE: mKey = HKEY_LOCAL_MACHINE; break;
                case RegKeyDirectory.HKEY_USERS: mKey = HKEY_USERS; break;
                case RegKeyDirectory.HKEY_CURRENT_CONFIG: mKey = HKEY_CURRENT_CONFIG; break;
            }
            var sKey = mKey.CreateSubKey(key.Name, true);
            if(key.Values != null && key.Values.Count > 0)
            {
                var nValues = WriteValuesInline(sKey, key.Values);
                node.Values = nValues;
            }
            if (key.Keys != null && key.Keys.Count > 0)
            {
                var nNodes = WriteKeysInline(sKey, key.Keys);
                node.Nodes = nNodes;
            }
            sKey.Close();
            return node;
        }

        public RegNode Read(RegNode node, RegKeyDirectory keyDir)
        {
            RegistryKey mKey = HKEY_CLASSES_ROOT;
            node.Type = keyDir;
            switch (keyDir)
            {
                case RegKeyDirectory.HKEY_CURRENT_USER: mKey = HKEY_CURRENT_USER; break;
                case RegKeyDirectory.HKEY_LOCAL_MACHINE: mKey = HKEY_LOCAL_MACHINE; break;
                case RegKeyDirectory.HKEY_USERS: mKey = HKEY_USERS; break;
                case RegKeyDirectory.HKEY_CURRENT_CONFIG: mKey = HKEY_CURRENT_CONFIG; break;
            }
            var sKey = mKey.OpenSubKey(node.Name);
            if (node.Values != null && node.Values.Count > 0)
            {
                var nValues = ReadValuesInline(sKey, node.Values);
                node.Values = nValues;
            }
            if (node.Nodes != null && node.Nodes.Count > 0)
            {
                var nNodes = ReadKeysInline(sKey, node.Nodes);
                node.Nodes = nNodes;
            }
            sKey.Close();
            return node;
        }

        public RegNode Remove(RegNode node, RegKeyDirectory keyDir)
        {
            RegistryKey mKey = HKEY_CLASSES_ROOT;
            node.Type = keyDir;
            switch (keyDir)
            {
                case RegKeyDirectory.HKEY_CURRENT_USER: mKey = HKEY_CURRENT_USER; break;
                case RegKeyDirectory.HKEY_LOCAL_MACHINE: mKey = HKEY_LOCAL_MACHINE; break;
                case RegKeyDirectory.HKEY_USERS: mKey = HKEY_USERS; break;
                case RegKeyDirectory.HKEY_CURRENT_CONFIG: mKey = HKEY_CURRENT_CONFIG; break;
            }

            if (node.Remove)
            {
                mKey.DeleteSubKeyTree(node.Name);
                return null;
            }

            var sKey = mKey.OpenSubKey(node.Name, true);
            if (node.Values != null && node.Values.Count > 0)
            {
                var nValues = RemoveValuesInline(sKey, node.Values);
                node.Values = nValues;
            }
            if (node.Nodes != null && node.Nodes.Count > 0)
            {
                var nNodes = RemoveKeysInline(sKey, node.Nodes);
                node.Nodes = nNodes;
            }
            sKey.Close();
            return node;
        }


        private List<RegNodeValue> WriteValuesInline(RegistryKey key, List<RegValue> values)
        {
            var result = new List<RegNodeValue>();
            foreach (RegValue value in values)
            {
                key.SetValue(value.Name, value.Value, value.Kind);
                result.Add(new RegNodeValue
                {
                    Name = value.Name,
                    Value = value.Value,
                    Kind = value.Kind
                });
            }
            return result;
        }
        private List<RegNode> WriteKeysInline(RegistryKey key, List<RegKey> keys)
        {
            var result = new List<RegNode>();
            foreach (RegKey r in keys)
            {
                var node = new RegNode
                {
                    Name = r.Name,
                    Type = RegKeyDirectory.Sub,
                };
                var sKey = key.CreateSubKey(r.Name, true);
                if(r.Values != null && r.Values.Count > 0)
                {
                    var nValues = WriteValuesInline(sKey, r.Values);
                    node.Values = nValues;
                }
                if(r.Keys != null && r.Keys.Count > 0)
                {
                    var nNodes = WriteKeysInline(sKey, r.Keys);
                    node.Nodes = nNodes;
                }
                sKey.Close();
                result.Add(node);
            }
            return result;
        }

        private List<RegNodeValue> ReadValuesInline(RegistryKey key, List<RegNodeValue> values)
        {
            var result = new List<RegNodeValue>();
            foreach (RegNodeValue value in values)
            {
                result.Add(new RegNodeValue
                {
                    Name = value.Name,
                    Value = key.GetValue(value.Name),
                    Kind = key.GetValueKind(value.Name)
                });
            }
            return result;
        }
        private List<RegNode> ReadKeysInline(RegistryKey key, List<RegNode> nodes)
        {
            var result = new List<RegNode>();
            foreach (RegNode r in nodes)
            {
                var sKey = key.OpenSubKey(r.Name);
                if (r.Values != null && r.Values.Count > 0)
                {
                    var nValues = ReadValuesInline(sKey, r.Values);
                    r.Values = nValues;
                }
                if (r.Nodes != null && r.Nodes.Count > 0)
                {
                    var nNodes = ReadKeysInline(sKey, r.Nodes);
                    r.Nodes = nNodes;
                }
                sKey.Close();
                result.Add(r);
            }
            return result;
        }

        private List<RegNodeValue> RemoveValuesInline(RegistryKey key, List<RegNodeValue> values)
        {
            var result = new List<RegNodeValue>();
            foreach (RegNodeValue value in values)
            {
                if(value.Remove)
                {
                    key.DeleteValue(value.Name);
                    continue;
                }
                result.Add(new RegNodeValue
                {
                    Name = value.Name,
                    Value = value.Value,
                    Kind = value.Kind
                });
            }
            return result;
        }
        private List<RegNode> RemoveKeysInline(RegistryKey key, List<RegNode> nodes)
        {
            var result = new List<RegNode>();
            foreach (RegNode r in nodes)
            {
                if (r.Remove)
                {
                    key.DeleteSubKeyTree(r.Name);
                    continue;
                }
                var node = new RegNode
                {
                    Name = r.Name,
                    Type = RegKeyDirectory.Sub,
                };
                var sKey = key.CreateSubKey(r.Name, true);
                if (r.Values != null && r.Values.Count > 0)
                {
                    var nValues = RemoveValuesInline(sKey, r.Values);
                    node.Values = nValues;
                }
                if (r.Nodes != null && r.Nodes.Count > 0)
                {
                    var nNodes = RemoveKeysInline(sKey, r.Nodes);
                    node.Nodes = nNodes;
                }
                sKey.Close();
                result.Add(node);
            }
            return result;
        }

        public static void InstallRegFile(string path)
        {
            path = path.Replace("\"", "");
            Process regeditProcess = Process.Start("regedit.exe", "/s \"" + path + "\"");
            regeditProcess.WaitForExit();
        }
    }
}