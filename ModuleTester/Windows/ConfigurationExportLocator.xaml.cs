using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace ModuleTester.Windows
{
    /// <summary>
    /// Interaction logic for ClaimFilesLocator.xaml
    /// </summary>
    public partial class ConfigurationLocator : Window
    {
        public string FileDirectory { get; set; }
        public bool Abort { get; set; }

        public ConfigurationLocator()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            FileDirectory = FileDirectoryTextBox.Text;
            Abort = false;
            Close();
        }

        private void SelectDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\Users";
            dialog.IsFolderPicker = false;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                FileDirectoryTextBox.Text = dialog.FileName;
                Abort = false;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Abort = true;
            Close();
        }
    }
}
