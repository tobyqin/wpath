using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            btnReset_Click(null, null);
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            this.txtPlace.Text = "c:\\test";
            this.lbTips.Content = "a sample tool for unit testing.";
            this.lbTips.Foreground = new SolidColorBrush(Colors.Black);
            this.lbTips.FontFamily = new FontFamily("Segoe UI");
            this.lbTips.FontSize = 11;

            this.txtId.Clear();
            this.txtId.Text = "cat";
            this.txtPassword.Clear();
            this.txtId.Clear();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            var info = string.Format("Key file was generated at {0}, please exit.", this.txtPlace.Text);
            LogInfo(info);
        }

        private void btnCleanup_Click(object sender, RoutedEventArgs e)
        {
            var f = this.txtPlace.Text;
            try
            {
                if (File.Exists(f))
                {
                    File.Delete(f);
                }
                LogInfo(f + " was removed!");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
            }
        }

        private void LogInfo(string info)
        {
            this.lbTips.Content = info;
            // mark as green.
            this.lbTips.Foreground = new SolidColorBrush(Colors.DarkGreen);
        }

        private void LogError(string info)
        {
            this.lbTips.Content = info;
            // mark as red!
            this.lbTips.Foreground = new SolidColorBrush(Colors.Red);
        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = "User.txt"; // Default file name
                dlg.DefaultExt = ".txt"; // Default file extension
                dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

                var dir = System.IO.Path.GetDirectoryName(this.txtPlace.Text);
                dlg.InitialDirectory = dir;

                // Show save file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process save file dialog box results
                if (result == true)
                {
                    // Save document
                    this.txtPlace.Text = dlg.FileName;
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
            }
        }
    }
}