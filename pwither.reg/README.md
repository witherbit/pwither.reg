# pwither.reg
## ������� ��� ������ � �������� Windows
���� ����� ������������ ��� ������� � �� �������������� ������ � �������� �� ������ ����������, ���� ���� ������ � �������� ������������� � �� ������������ ������� �����������, �� ����� �������������� Microsoft.Win32.Registry.
> ��� ������ � �������� ��������� ����� ��������������! ������� ����������� ������������ ��� .NET 7.0

#### ������������ ����
```csharp
using pwitherrgx;
using pwitherrgx.Enums;
using pwitherrgx.Objects;
using pwitherrgx.Utils;
```
#### ������ � ������ �� ����� .reg
����������� ����������� �����:
```csharp
Reg.InstallRegFile(string path);
```
������ �������������:
```csharp
Reg.InstallRegFile("\"C:\\Users\\Witherbit\\Desktop\\links.reg\"");
```
#### RegKey � RegNode
��� ���������� �������� ������ ������������ RegKey:
```csharp
var key = new RegKey
{
    Name = "Software",
    Keys = new()
    {
        new RegKey
        {
            Name = "Witherbit",
            Keys = new()
            {
                new RegKey
                {
                    Name = "Test",
                    Values = new()
                    {
                        new RegValue("ver", "1.0.0")
                    }
                },
                new RegKey
                {
                    Name = "Removation"
                }
            }
        }
    }
};
```
��� ����� ��������������� ������������:
```csharp
var key = "Software/Witherbit"
    .ToRegKey()
    .AddChildRegKey("Test", 0, new()
        {
            new RegValue("ver", "1.0.0")
        })
    .AddChildRegKey("Removation", 0);
```
��� ���������� �������� ������ � �������� ������������ RegNode:
```csharp
//������� ����������: RegNode(��� �����, RegNodeValues, RegNodes, ������� �� ��� ������ ��������)
var node = new RegNode("Software", null, new()
{
    new RegNode("Witherbit", null, new()
    {
        new RegNode("Test", new() { new RegNodeValue("ver", Microsoft.Win32.RegistryValueKind.String) }, null),
        new RegNode("Removation", null, null, true)
    })
});
```
��� ����� ��������������� ������������:
```csharp
var node = "Software/Witherbit"
    .ToRegNode()
    .AddChildRegNode("Test", 0, new()
        {
            new RegNodeValue("ver")
        })
    .AddChildRegNode("Removation", 0, remove: RegRemoveType.RemoveLast);
```
#### Reg - ������ � ��������
������
```csharp
Reg reg = new Reg();
var key = "Software/Witherbit"
    .ToRegKey()
    .AddChildRegKey("Test", 0, new()
        {
            new RegValue("ver", "1.0.0")
        })
    .AddChildRegKey("Removation", 0);

var node = reg.Write(key, RegKeyDirectory.HKEY_CURRENT_USER);
```
������
```csharp
Reg reg = new Reg();
var node = "Software/Witherbit"
    .ToRegNode()
    .AddChildRegNode("Test", 0, new()
        {
            new RegNodeValue("ver")
        })
    .AddChildRegNode("Removation", 0);

reg.Read(node, RegKeyDirectory.HKEY_CURRENT_USER); //����� ���������� ��������� ��� RegNode

Console.WriteLine(JsonConvert.SerializeObject(node, Formatting.Indented)); // ��������� Newtonsoft.Json ��� ������������ ��������
```
�������� ������ � ��������� (�������� Remove)
```csharp
Reg reg = new Reg();
var node = "Software/Witherbit"
    .ToRegNode()
    .AddChildRegNode("Test", 0, new()
        {
            new RegNodeValue("ver")
        })
    .AddChildRegNode("Removation", 0, remove: RegRemoveType.RemoveLast); //������� ������ ����� Removation, ��� ����� ����������� �������� ������� ������������ ����������� ���������� RegNode (��� ����������)

reg.Remove(node, RegKeyDirectory.HKEY_CURRENT_USER);

Console.WriteLine(JsonConvert.SerializeObject(node, Formatting.Indented));
```
������ �������� �����
```csharp
Reg reg = new Reg();
var node = "Software"
    .ToRegNode()
    .AddRegNode("Sample", remove: RegRemoveType.RemoveAll); //���� ���� �� �������� � ���� Sample ������ ����� ��� ��������, ��� ����� ������� (RemoveAll)

reg.Remove(node, RegKeyDirectory.HKEY_CURRENT_USER);
```
##### ����� �� ����� ��������������� �� ��������� ������ �������, ���������� �� ���� ����� � ����
