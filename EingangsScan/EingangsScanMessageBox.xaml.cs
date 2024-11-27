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
            dialogOption = input;
            InitializeComponent();
            UpdateMessageText();
            AbbrechenKnopf.Focus();


            DataContext = this;

        }

        private void UpdateMessageText()
        {
            //Auftrag muss storniert werden
            if (dialogOption == EingangsMessageBox.stornieren)
            {
                setUiToStorno();
            }
            //Auftrag muss reaktivier werden
            else if (dialogOption == EingangsMessageBox.reaktivieren)
            {
                
                setUiToRaktivierung();
            }
            //Fehler 
            else
            {
                DialogResult = false;
            }
        }

        private void setUiToStorno()
        {
            BtnReaktivieren.Visibility = Visibility.Hidden;
            BtnStorno.Visibility = Visibility.Visible;
            ReaktivierenmeldungUpper.Visibility = Visibility.Hidden;
            ReaktivierenmeldungLower.Visibility = Visibility.Hidden;
            StornomeldungLower.Visibility = Visibility.Visible;
            StornomeldungUpper.Visibility = Visibility.Visible;
        }
        private void setUiToRaktivierung()
        {
            BtnReaktivieren.Visibility = Visibility.Visible;
            BtnStorno.Visibility = Visibility.Hidden;
            ReaktivierenmeldungUpper.Visibility = Visibility.Visible;
            ReaktivierenmeldungLower.Visibility = Visibility.Visible;
            StornomeldungLower.Visibility = Visibility.Hidden;
            StornomeldungUpper.Visibility = Visibility.Hidden;
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
