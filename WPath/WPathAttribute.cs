using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPath
{
    /// <summary>
    /// The WPath looks like XPath for web element, it is not exactly equal to XPath.
    /// For more info please refer to https://github.com/tobyqin/wpath
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public class WPathAttribute : Attribute
    {
        public WPathAttribute(string path)
        {
            this.Path = path;
        }

        public string Path { get; set; }
    }
}