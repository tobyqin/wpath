# WPath Introduction
WPath is a library to select Windows UI automation element like XPath. The WPath looks like [XPath][1] which was wildly using to select xml elements, but it is not exactly equal to [XPath][1],  it is being used to locate [Microsoft UIAutomation][2] elements. Some special rules list below.

Tags: UIA, UIPath, UIAPath, UILocator

1. The path should be started with '/'.
2. Use '/' to separate element nodes in a path.
3. The node name should be [control type][3] name, but it's optional.
4. The UI element properties are treated as attribute in XPath.
5. Supported attributes:
  + `Name` (NameProperty)
  + `ID` (AutomationIdProperty)
  + `Class` (ClassNameProperty)
  + `Enabled` (IsEnabledProperty)
  + `FrameworkID` (FrameworkIdProperty)

### Examples:

> `/Group/Button`
  + Find the first button under first group element.

> `//Button[@Name='Save']`
  + Find a button with name "Save" in descendants.

> `/[@Name='TabContainer']/Button[2]`
  + Find the second button under an element named with 'TabContainer'

> `/Button[@ID='AddButton' and @Name='Add']`
  + Find a button with automation ID 'AddButton' **and** name 'Add'

> `/Button[@ID='AddButton' or @Name='Add']`
  + Find a button with automation ID 'AddButton' **or** name 'Add'

> `/Button[first()]`
  + Find the first button under current node

> `/Button[last()]`
  + Find the last button under current node

### Usage

1. Set WPath by attribute, works for C# function and property member.

```cs
[WPath("/Edit[@id='txtSeoid' or @Class='TextBox']")]
public AutomationElement EditControl
{
   get { return this.AppElement.FindByWPath(); }
}

[WPath("/Button[first()]")]
public AutomationElement GetFirstButton()
{
   return this.AppElement.FindByWPath();
}
```

2. Call `FindByWPath(path)` method to locate the element

```cs
var path = "/Edit[3]";
var e = this.AppElement.FindByWPath(path);
Assert.AreEqual("txtKey", e.Current.AutomationId);
Assert.AreEqual(ControlType.Edit, e.Current.ControlType);

path = "/Button[@name='OK']/Text[1]";
e = this.AppElement.FindByWPath(path);
Assert.AreEqual("OK", e.Current.Name);
Assert.AreEqual(ControlType.Text, e.Current.ControlType);
```

### Tips:
- The node name and attribute name is case insensitive
  - @name = @Name
  - /edit = /Edit
- Welcome to extend the feature by sending pull requests.
- Parent element locator `../` is **not** support yet.

[1]: http://www.w3schools.com/xsl/xpath_intro.asp
[2]: https://msdn.microsoft.com/en-us/library/ms747327(v=vs.110).aspx
[3]: https://msdn.microsoft.com/en-us/library/ms743581(v=vs.110).aspx
