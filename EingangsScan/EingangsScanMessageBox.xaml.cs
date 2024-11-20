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
        private string _Message { get; set; }

        EingangsMessageBox dialogOption;


        public EingangsScanMessageBox(EingangsMessageBox input)
        {
            InitializeComponent();


            dialogOption = input;
            DataContext = this;
        }

        private void SetMessageText(EingangsMessageBox input)
        {
            //Auftrag muss storniert werden
            if(dialogOption == EingangsMessageBox.stornieren )
            {
                _Message = "Auftrag für die Montage sperren?";
                setUiToStorno();
            }
            //Auftrag muss reaktivier werden
            else if(dialogOption == EingangsMessageBox.reaktivieren)
            {
                _Message = "Auftrag wurde gesperrt. \n Möchten Sie ihn reaktivieren?";
                setUiToRaktivierung();
            }
            //Fehler 
            else 
            {
                dialogOption = EingangsMessageBox.fehler;
            }
        }

        private void setUiToStorno()
        {
            BtnReaktivieren.Visibility = Visibility.Collapsed;
            BtnStorno.Visibility = Visibility.Visible;
        }
        private void setUiToRaktivierung()
        {
            BtnReaktivieren.Visibility = Visibility.Visible;
            BtnStorno.Visibility = Visibility.Collapsed;
        }



        private void Button_Storno(object sender, RoutedEventArgs e)
        {
            dialogOption = EingangsMessageBox.stornieren;
            DialogResult = true;
        }
        private void Button_Close(object sender, RoutedEventArgs e)
        {
            dialogOption = EingangsMessageBox.abbrechen;
            DialogResult = false;
        }
        private void Button_Reaktivieren(object sender, RoutedEventArgs e)
        {
            dialogOption = EingangsMessageBox.reaktivieren;
            DialogResult = true;
        }
    }
}
