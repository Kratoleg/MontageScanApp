using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EingangsScan
{
    /// <summary>
    /// Interaktionslogik für EingangsScanMessageBox.xaml
    /// </summary>
    public partial class EingangsScanMessageBox : Window
    {
        private string Message { get; set; }

        public EingangsScanMessageBox(string message)
        {
            InitializeComponent();
            Message = message;
            DataContext = this;
        }


        private void Button_Storno(object sender, RoutedEventArgs e)
        {

        }
        private void Button_Close(object sender, RoutedEventArgs e)
        {

        }
        private void Button_Reaktivieren(object sender, RoutedEventArgs e)
        {

        }
    }
}
