using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace WPath
{
    internal class WPathFinder
    {
        public Dictionary<string, ControlType> ControlTypes { get; set; }

        public WPathFinder()
        {
            ControlTypes = new Dictionary<string, ControlType>();
            ControlTypes.Add("button", ControlType.Button);
            ControlTypes.Add("calendar", ControlType.Calendar);
            ControlTypes.Add("checkbox", ControlType.CheckBox);
            ControlTypes.Add("combobox", ControlType.ComboBox);
            ControlTypes.Add("custom", ControlType.Custom);
            ControlTypes.Add("datagrid", ControlType.DataGrid);
            ControlTypes.Add("dataitem", ControlType.DataItem);
            ControlTypes.Add("document", ControlType.Document);
            ControlTypes.Add("edit", ControlType.Edit);
            ControlTypes.Add("group", ControlType.Group);
            ControlTypes.Add("header", ControlType.Header);
            ControlTypes.Add("headeritem", ControlType.HeaderItem);
            ControlTypes.Add("hyperlink", ControlType.Hyperlink);
            ControlTypes.Add("image", ControlType.Image);
            ControlTypes.Add("list", ControlType.List);
            ControlTypes.Add("listitem", ControlType.ListItem);
            ControlTypes.Add("menu", ControlType.Menu);
            ControlTypes.Add("menubar", ControlType.MenuBar);
            ControlTypes.Add("menuitem", ControlType.MenuItem);
            ControlTypes.Add("pane", ControlType.Pane);
            ControlTypes.Add("progressbar", ControlType.ProgressBar);
            ControlTypes.Add("radiobutton", ControlType.RadioButton);
            ControlTypes.Add("scrollbar", ControlType.ScrollBar);
            ControlTypes.Add("separator", ControlType.Separator);
            ControlTypes.Add("slider", ControlType.Slider);
            ControlTypes.Add("spinner", ControlType.Spinner);
            ControlTypes.Add("splitbutton", ControlType.SplitButton);
            ControlTypes.Add("statusbar", ControlType.StatusBar);
            ControlTypes.Add("tab", ControlType.Tab);
            ControlTypes.Add("tabitem", ControlType.TabItem);
            ControlTypes.Add("table", ControlType.Table);
            ControlTypes.Add("text", ControlType.Text);
            ControlTypes.Add("thumb", ControlType.Thumb);
            ControlTypes.Add("titlebar", ControlType.TitleBar);
            ControlTypes.Add("toolbar", ControlType.ToolBar);
            ControlTypes.Add("tooltip", ControlType.ToolTip);
            ControlTypes.Add("tree", ControlType.Tree);
            ControlTypes.Add("treeitem", ControlType.TreeItem);
            ControlTypes.Add("window", ControlType.Window);
        }

        /// <summary>
        /// Find an element by UI Path.
        /// </summary>
        /// <param name="currentElement">Current element.</param>
        /// <param name="fullPath">Full path of the UI element.</param>
        /// <returns>The element to be found.</returns>
        public AutomationElement FindElement(AutomationElement currentElement, string fullPath)
        {
            if (fullPath[0] != '/')
            {
                throw new ArgumentException(fullPath);
            }

            // "\/" in full path will be treated as escaped char '/'
            // empty element in the node list are separators
            var elementNodes = fullPath.Replace("/", "#|#")
                .Replace(@"\#|#", "/")
                .Split(new string[] { "#|#" }, StringSplitOptions.None);

            var nodePath = string.Empty;

            for (int i = 1; i < elementNodes.Length; i++)
            {
                if (elementNodes[i].IsNullOrEmpty())
                {
                    nodePath = elementNodes[i + 1];
                    currentElement = FindNode(currentElement, TreeScope.Descendants, nodePath);
                    i = i + 1;
                }
                else
                {
                    nodePath = elementNodes[i];
                    currentElement = FindNode(currentElement, TreeScope.Children, nodePath);
                }
            }

            return currentElement;
        }

        private AutomationElement FindNode(AutomationElement currentElement, TreeScope scope, string nodePath)
        {
            UIProperty uip = null;
            var condition = ConvertFromPath(nodePath, out uip);
            if (uip == null)
            {
                return currentElement.FindFirst(scope, condition);
            }
            else
            {
                var all = currentElement.FindAll(scope, condition);
                if (uip.Type == UIProperty.Types.First)
                {
                    return all.Cast<AutomationElement>().First();
                }
                else if (uip.Type == UIProperty.Types.Index)
                {
                    return all[uip.Index - 1];
                }
                else if (uip.Type == UIProperty.Types.Last)
                {
                    return all.Cast<AutomationElement>().Last();
                }
                else if (uip.Type == UIProperty.Types.Empty)
                {
                    return all.Cast<AutomationElement>().First();
                }
                else
                {
                    throw new NotSupportedException("Invalid node: {0}".FormatWith(nodePath));
                }
            }
        }

        /// <summary>
        /// Convert element path to UI search condition.
        /// If the path needs parent element to locate target element
        /// We will return the UIProperty instead.
        /// </summary>
        /// <param name="nodePath"></param>
        /// <returns></returns>
        private Condition ConvertFromPath(string nodePath, out UIProperty uip)
        {
            var pattern = @"^(?<type>[^\[\]]+)*(\[(?<property>.*)\])*$";
            var match = Regex.Match(nodePath, pattern);

            if (match.Success)
            {
                var controlName = match.Groups["type"].Value.ToLower();
                var properties = match.Groups["property"].Value;

                var propCondition = BuildPropertyCondition(properties, out uip);
                var controlCondition = BuildControlCondition(controlName);

                if (uip != null)
                {
                    if (uip.Type == UIProperty.Types.NameValue)
                    {
                        uip = null;
                        return new AndCondition(controlCondition, propCondition);
                    }
                    else
                    {
                        return controlCondition;
                    }
                }
                else
                {
                    return new AndCondition(controlCondition, propCondition);
                }
            }
            else
            {
                throw new ArgumentException("Invalid UI Path: '{0}'".FormatWith(nodePath));
            }
        }

        private Condition BuildControlCondition(string controlName)
        {
            if (!controlName.IsNullOrEmpty())
            {
                return new PropertyCondition(
                    AutomationElement.ControlTypeProperty, ControlTypes[controlName]);
            }
            else
            {
                return Condition.TrueCondition;
            }
        }

        /// <summary>
        /// Format as:
        /// [@Name='name' and @Id='id']
        /// [@Name='name' or @Id='id']
        /// [1]
        /// [first()]
        /// [last()]
        /// Only support: Name, ID, Class, Enabled, FrameworkID
        /// </summary>
        /// <param name="property">The property string.</param>
        /// <returns>The condition</returns>
        private Condition BuildPropertyCondition(string property, out UIProperty uip)
        {
            // use the pattern to find operator (and|or) in property string
            var op = @"[')]+(?<opfull>[ ]*(?<op>and|or)[ ]*)[fl@]+";
            var match = Regex.Match(property, op, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                // search the first property
                var isOrCondition = match.Groups["op"].Value.ToLower() == "or";
                var opFullText = match.Groups["opfull"].Value;
                var firstProperty = property.Substring(0, property.IndexOf(opFullText));

                // trim the first property to find next property
                var subProperty = property.Substring((firstProperty + opFullText).Length);

                // try to find the second property
                match = Regex.Match(subProperty, op, RegexOptions.IgnoreCase);
                var secondProperty = string.Empty;
                var haveMoreProperty = false;
                var isMoreOrCondition = false;

                // if have more property, we should know it is OR/AND
                if (match.Success)
                {
                    opFullText = match.Groups["opfull"].Value;
                    secondProperty = subProperty.Substring(0, subProperty.IndexOf(opFullText));
                    isMoreOrCondition = match.Groups["op"].Value.ToLower() == "or";
                    haveMoreProperty = true;
                }
                else
                {
                    // use the entire string as second property
                    secondProperty = subProperty;
                }

                // combine the conditions
                var firstCondition = new UIProperty(firstProperty).ConvertToCondition();
                var secondCondition = new UIProperty(secondProperty).ConvertToCondition();
                Condition currentCondition = null;

                if (isOrCondition)
                {
                    currentCondition = new OrCondition(firstCondition, secondCondition);
                }
                else
                {
                    currentCondition = new AndCondition(firstCondition, secondCondition);
                }

                if (haveMoreProperty)
                {
                    if (isMoreOrCondition)
                    {
                        uip = null;
                        return new OrCondition(currentCondition,
                            BuildPropertyCondition(subProperty, out uip));
                    }
                    else
                    {
                        uip = null;
                        return new AndCondition(currentCondition,
                            BuildPropertyCondition(subProperty, out uip));
                    }
                }
                else
                {
                    uip = null;
                    return currentCondition;
                }
            }
            else
            {
                uip = new UIProperty(property);
                return uip.ConvertToCondition();
            }
        }
    }
}