using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ADBFastbootGUI.Windows
{
    /// <summary>
    /// Interaction logic for ProgramUninstallDialog.xaml
    /// </summary>
    public partial class ProgramUninstallDialog : Window
    {
        MainWindow mw = new MainWindow();
        string adbpath = $@"C:\Program Files\ADBFastbootGUI\";
        public ProgramUninstallDialog()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void UninstallAppButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
