using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WPath;

namespace WPath.Tests
{
    [TestClass]
    public class UiaTests
    {
        [WPath("/Edit[@id='txtId' or @Class='TextBox']")]
        public AutomationElement EditControl
        {
            get { return AppElement.FindByWPath(); }
        }

        [WPath("/Button[first()]")]
        public AutomationElement GetFirstButton()
        {
            return AppElement.FindByWPath();
        }

        public static AutomationElement AppElement { get; set; }

        public static Process AppProcess { get; set; }

        [ClassInitialize]
        public static void Setup(TestContext testContext)
        {
            var appPath = @"..\..\..\TestApp\bin\Debug\TestApp.exe";
            Debug.WriteLine(Environment.CurrentDirectory);
            Assert.IsTrue(File.Exists(appPath));
            AppProcess = Process.Start(appPath);
            Thread.Sleep(1000);
            AppElement = AutomationElement.RootElement.FindChildByName("Generate User Key", ControlType.Window);
            Assert.IsNotNull(AppElement);
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            // kill the app.
            AppProcess.Kill();
        }

        [TestMethod]
        public void Child_TypeOnly()
        {
            var path = "/Text";
            var e = AppElement.FindByWPath(path);
            Assert.AreEqual("ID: ", e.Current.Name);
            Assert.AreEqual(ControlType.Text, e.Current.ControlType);
        }

        [TestMethod]
        public void Child_OneNode()
        {
            var path = "/Text[@Name='ID: ']";
            var e = AppElement.FindByWPath(path);
            Assert.AreEqual("ID: ", e.Current.Name);
            Assert.AreEqual(ControlType.Text, e.Current.ControlType);
        }

        [TestMethod]
        public void Child_AttributeOnly()
        {
            var path = "/[@Name='ID: ']";
            var e = AppElement.FindByWPath(path);
            Assert.AreEqual("ID: ", e.Current.Name);
            Assert.AreEqual(ControlType.Text, e.Current.ControlType);
        }

        [TestMethod]
        public void Child_AndProperty()
        {
            var path = "/Edit[@id='txtId' and @Class='TextBox']";
            var e = AppElement.FindByWPath(path);
            Assert.AreEqual("txtId", e.Current.AutomationId);
            Assert.AreEqual(ControlType.Edit, e.Current.ControlType);
        }

        [TestMethod]
        public void Child_OrProperty()
        {
            var path = "/Edit[@id='txtId' or @Class='TextBox']";
            var e = AppElement.FindByWPath(path);
            Assert.AreEqual("txtId", e.Current.AutomationId);
            Assert.AreEqual(ControlType.Edit, e.Current.ControlType);
        }

        [TestMethod]
        public void Child_First()
        {
            var path = "/Edit[first()]";
            var e = AppElement.FindByWPath(path);
            Assert.AreEqual("txtId", e.Current.AutomationId);
            Assert.AreEqual(ControlType.Edit, e.Current.ControlType);

            path = "/Button[first()]";
            e = AppElement.FindByWPath(path);
            Assert.AreEqual("...", e.Current.Name);
            Assert.AreEqual(ControlType.Button, e.Current.ControlType);

            path = "/Text[first()]";
            e = AppElement.FindByWPath(path);
            Assert.AreEqual("ID: ", e.Current.Name);
            Assert.AreEqual(ControlType.Text, e.Current.ControlType);
        }

        [TestMethod]
        public void ChildLast()
        {
            var path = "/Edit[last()]";
            var e = AppElement.FindByWPath(path);
            Assert.AreEqual("txtPlace", e.Current.AutomationId);
            Assert.AreEqual(ControlType.Edit, e.Current.ControlType);
        }

        [TestMethod]
        public void Child_Index()
        {
            var path = "/Edit[1]";
            var e = AppElement.FindByWPath(path);
            Assert.AreEqual("txtId", e.Current.AutomationId);
            Assert.AreEqual(ControlType.Edit, e.Current.ControlType);

            path = "/Edit[2]";
            e = AppElement.FindByWPath(path);
            Assert.AreEqual("txtPassword", e.Current.AutomationId);
            Assert.AreEqual(ControlType.Edit, e.Current.ControlType);

            path = "/Edit[3]";
            e = AppElement.FindByWPath(path);
            Assert.AreEqual("txtPlace", e.Current.AutomationId);
            Assert.AreEqual(ControlType.Edit, e.Current.ControlType);

            path = "/Button[@name='OK']/Text[1]";
            e = AppElement.FindByWPath(path);
            Assert.AreEqual("OK", e.Current.Name);
            Assert.AreEqual(ControlType.Text, e.Current.ControlType);
        }

        [TestMethod]
        public void Child_DeepNode()
        {
            var path = "/Window[@Name='Generate User Key']/Button[@name='OK']/Text[@name='OK']";
            Assert.AreEqual(ControlType.Pane, AutomationElement.RootElement.Current.ControlType);
            var e = AutomationElement.RootElement.FindByWPath(path);
            Assert.AreEqual("OK", e.Current.Name);
            Assert.AreEqual(ControlType.Text, e.Current.ControlType);

            path = "/Button[@name = 'OK']/Text[@name='OK']";
            e = AppElement.FindByWPath(path);
            Assert.AreEqual("OK", e.Current.Name);
            Assert.AreEqual(ControlType.Text, e.Current.ControlType);

            path = "/Button[@name = 'OK' ]/Text[@name='OK']";
            e = AppElement.FindByWPath(path);
            Assert.AreEqual("OK", e.Current.Name);
            Assert.AreEqual(ControlType.Text, e.Current.ControlType);

            path = "/Button[@name = 'OK']/Text[@name = 'OK']";
            e = AppElement.FindByWPath(path);
            Assert.AreEqual("OK", e.Current.Name);
            Assert.AreEqual(ControlType.Text, e.Current.ControlType);

            path = "/Button[@name='OK']/Text[@name='OK']";
            e = AppElement.FindByWPath(path);
            Assert.AreEqual("OK", e.Current.Name);
            Assert.AreEqual(ControlType.Text, e.Current.ControlType);
        }

        [TestMethod]
        public void Chlid_MultipleAttribute()
        {
            var path = "/Edit[@name='test' or @id='test' or @enabled='true']";
            var e = AppElement.FindByWPath(path);
            Assert.AreEqual("txtId", e.Current.AutomationId);
            Assert.AreEqual(ControlType.Edit, e.Current.ControlType);

            path = "/Edit[@name='test' or @id='txtId' and @enabled='true']";
            e = AppElement.FindByWPath(path);
            Assert.AreEqual("txtId", e.Current.AutomationId);
            Assert.AreEqual(ControlType.Edit, e.Current.ControlType);

            path = "/Edit[@id='txtId' and @enabled='true' and @framework='WPF']";
            e = AppElement.FindByWPath(path);
            Assert.AreEqual("txtId", e.Current.AutomationId);
            Assert.AreEqual(ControlType.Edit, e.Current.ControlType);
        }

        [TestMethod]
        public void Chlid_MultipleAttribute_WithoutType()
        {
            var path = "/[@name='test' or @id='test' or @enabled='true']";
            var e = AppElement.FindByWPath(path);
            Assert.AreEqual("TitleBar", e.Current.AutomationId);
            Assert.AreEqual(ControlType.TitleBar, e.Current.ControlType);

            path = "/[@name='test' or @id='txtId' and @enabled='true']";
            e = AppElement.FindByWPath(path);
            Assert.AreEqual("txtId", e.Current.AutomationId);
            Assert.AreEqual(ControlType.Edit, e.Current.ControlType);

            path = "/[@id='txtId' and @enabled='true' and @framework='WPF']";
            e = AppElement.FindByWPath(path);
            Assert.AreEqual("txtId", e.Current.AutomationId);
            Assert.AreEqual(ControlType.Edit, e.Current.ControlType);
        }

        [TestMethod]
        public void Child_AsDescendants()
        {
            var path = "//Edit[@name='test' or @id='test' or @enabled='true']";
            var e = AppElement.FindByWPath(path);
            Assert.AreEqual("txtId", e.Current.AutomationId);
            Assert.AreEqual(ControlType.Edit, e.Current.ControlType);

            path = "//Edit[@name='test' or @id='txtId' and @enabled='true']";
            e = AppElement.FindByWPath(path);
            Assert.AreEqual("txtId", e.Current.AutomationId);
            Assert.AreEqual(ControlType.Edit, e.Current.ControlType);

            path = "//Edit[@id='txtId' and @enabled='true' and @framework='WPF']";
            e = AppElement.FindByWPath(path);
            Assert.AreEqual("txtId", e.Current.AutomationId);
            Assert.AreEqual(ControlType.Edit, e.Current.ControlType);
        }

        [TestMethod]
        public void Descendants_Attribute()
        {
            var path = "//Text[@name='OK']";
            var e = AppElement.FindByWPath(path);
            Assert.AreEqual("OK", e.Current.Name);
            Assert.AreEqual(ControlType.Text, e.Current.ControlType);
        }

        [TestMethod]
        public void PathAttribute_OnProperty()
        {
            var e = this.EditControl;
            Assert.AreEqual("txtId", e.Current.AutomationId);
            Assert.AreEqual(ControlType.Edit, e.Current.ControlType);
        }

        [TestMethod]
        public void PathAttribute_OnMethod()
        {
            var e = GetFirstButton();
            Assert.AreEqual("...", e.Current.Name);
            Assert.AreEqual(ControlType.Button, e.Current.ControlType);
        }
    }
}