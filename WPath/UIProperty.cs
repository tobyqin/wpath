using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace WPath
{
    internal class UIProperty
    {
        public enum Types
        {
            Invalid,
            NameValue,
            Index,
            First,
            Last,
            Empty
        }

        private string nameValuePattern = @"@(?<name>\w+)[ ]*=[ ]*'(?<value>.+)'";
        private string indexPattern = @"(?<index>\d)";
        private string firstElementPattern = @"(?<func>[ ]*first\(\)[ ]*)";
        private string lastElementPattern = @"(?<func>[ ]*last\(\)[ ]*)";

        /// <summary>
        /// Property string is a text like: @Name='name'
        /// </summary>
        /// <param name="propertyString"></param>
        public UIProperty(string propertyString)
        {
            this.Text = propertyString;
            this.Parse();
        }

        public Types Type { get; set; }

        public string Text { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public int Index { get; set; }

        public bool SelectFirst { get; set; }

        public bool SelectLast { get; set; }

        public Condition ConvertToCondition()
        {
            switch (this.Type)
            {
                case Types.Empty:
                    return Condition.TrueCondition;

                case Types.NameValue:
                    return ConvertNameValueProperty();

                case Types.Index:
                    return Condition.TrueCondition;

                case Types.First:
                    return Condition.TrueCondition;

                case Types.Last:
                    return Condition.TrueCondition;

                default:
                    throw new ArgumentException("Invalid property: {0}".FormatWith(this.Text));
            }
        }

        private Condition ConvertNameValueProperty()
        {
            switch (this.Name.ToLower())
            {
                case "name":
                    return new PropertyCondition(AutomationElement.NameProperty, this.Value);

                case "id":
                    return new PropertyCondition(AutomationElement.AutomationIdProperty, this.Value);

                case "class":
                    return new PropertyCondition(AutomationElement.ClassNameProperty, this.Value);

                case "framework":
                    return new PropertyCondition(AutomationElement.FrameworkIdProperty, this.Value);

                case "enabled":
                    var enabled = Convert.ToBoolean(this.Value);
                    return new PropertyCondition(AutomationElement.IsEnabledProperty, enabled);

                default:
                    throw new ArgumentException("Unkown property: {0}".FormatWith(this.Text));
            }
        }

        /// <summary>
        /// Parse a property string to UIProperty.
        /// </summary>
        private void Parse()
        {
            this.Type = Types.Invalid;

            if (this.Text.IsNullOrEmpty())
            {
                this.Type = Types.Empty;
                return;
            }

            var match = Regex.Match(this.Text, nameValuePattern);
            if (match.Success)
            {
                this.Type = Types.NameValue;
                this.Name = match.Groups["name"].Value;
                this.Value = match.Groups["value"].Value;
                return;
            }

            match = Regex.Match(this.Text, indexPattern);
            if (match.Success)
            {
                this.Type = Types.Index;
                this.Index = Convert.ToInt32(match.Groups["index"].Value);
                return;
            }

            match = Regex.Match(this.Text, firstElementPattern);
            if (match.Success)
            {
                this.Type = Types.First;
                this.SelectFirst = true;
            }

            match = Regex.Match(this.Text, lastElementPattern);
            if (match.Success)
            {
                this.Type = Types.Last;
                this.SelectLast = true;
            }
        }
    }
}