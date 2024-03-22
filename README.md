# pwither.reg
## Утилита для работы с реестром Windows
Этот пакет предназначен для сложной и не единовременной работы с реестром из вашего приложения, если ваша работа с реестром единовременна и не представляет сложных конструкций, то лучше воспользуйтесь Microsoft.Win32.Registry.
> Для работы с реестром требуются права администратора! Краткое руководство представлено для .NET 7.0

#### Пространства имен
```csharp
using pwither.reg;
using pwither.reg.Enums;
using pwither.reg.Objects;
using pwither.reg.Utils;
```
#### Запись в реестр из файла .reg
Используйте статический метод:
```csharp
Reg.InstallRegFile(string path);
```
Пример использования:
```csharp
Reg.InstallRegFile("\"C:\\Users\\Witherbit\\Desktop\\links.reg\"");
```
#### RegKey и RegNode
Для построения запросов записи используется RegKey:
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
Или можно воспользоваться расширениями:
```csharp
var key = "Software/Witherbit"
    .ToRegKey()
    .AddChildRegKey("Test", 0, new()
        {
            new RegValue("ver", "1.0.0")
        })
    .AddChildRegKey("Removation", 0);
```
Для построения запросов чтения и удаления используется RegNode:
```csharp
//порядок параметров: RegNode(Имя ключа, RegNodeValues, RegNodes, Удалять ли при методе удаления)
var node = new RegNode("Software", null, new()
{
    new RegNode("Witherbit", null, new()
    {
        new RegNode("Test", new() { new RegNodeValue("ver", Microsoft.Win32.RegistryValueKind.String) }, null),
        new RegNode("Removation", null, null, true)
    })
});
```
Или можно воспользоваться расширениями:
```csharp
var node = "Software/Witherbit"
    .ToRegNode()
    .AddChildRegNode("Test", 0, new()
        {
            new RegNodeValue("ver")
        })
    .AddChildRegNode("Removation", 0, remove: RegRemoveType.RemoveLast);
```
#### Reg - работа с реестром
Запись
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
Чтение
```csharp
Reg reg = new Reg();
var node = "Software/Witherbit"
    .ToRegNode()
    .AddChildRegNode("Test", 0, new()
        {
            new RegNodeValue("ver")
        })
    .AddChildRegNode("Removation", 0);

reg.Read(node, RegKeyDirectory.HKEY_CURRENT_USER); //также возвращает ссылочный тип RegNode

Console.WriteLine(JsonConvert.SerializeObject(node, Formatting.Indented)); // требуется Newtonsoft.Json для сериализации объектов
```
Удаление ключей и элементов (Свойство Remove)
```csharp
Reg reg = new Reg();
var node = "Software/Witherbit"
    .ToRegNode()
    .AddChildRegNode("Test", 0, new()
        {
            new RegNodeValue("ver")
        })
    .AddChildRegNode("Removation", 0, remove: RegRemoveType.RemoveLast); //удаляет только папку Removation, для более продвинутой настроки следует использовать стандартное объявление RegNode (Без расширений)

reg.Remove(node, RegKeyDirectory.HKEY_CURRENT_USER);

Console.WriteLine(JsonConvert.SerializeObject(node, Formatting.Indented));
```
Полное удаление ключа
```csharp
Reg reg = new Reg();
var node = "Software"
    .ToRegNode()
    .AddRegNode("Sample", remove: RegRemoveType.RemoveAll); //даже если вы добавите в ключ Sample другие ключи или значения, они будут удалены (RemoveAll)

reg.Remove(node, RegKeyDirectory.HKEY_CURRENT_USER);
```
##### Автор не несет ответственности за изменения вашего реестра, действуйте на свой страх и риск
