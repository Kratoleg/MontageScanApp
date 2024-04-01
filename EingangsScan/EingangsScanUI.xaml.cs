using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
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
using MontageScanLib.Models;
using MontageScanDataAccessLib;
using MontageScanLib;




namespace EingangsScan;

public partial class EingangsScanUI : Window
{
    BindingList<EingangsLieferscheinModel> angezeigteLieferscheine = new BindingList<EingangsLieferscheinModel>();
    SqlLieferschein sqlLieferschein;
    public EingangsScanUI()
    {
        InitializeComponent();
        sqlLieferschein = new SqlLieferschein(GetConnectionString("PrivateMontageScan"));
        AuftragsListe.ItemsSource = angezeigteLieferscheine;
    }

    private string GetConnectionString(string name)
    {
        ConnectionStringSettings? settings =
            ConfigurationManager.ConnectionStrings[name];
        return settings?.ConnectionString;
    }

    private void AuftragsListe_Loaded(object sender, RoutedEventArgs e)
    {
        if (AuftragsListe.Items is INotifyCollectionChanged collection)
        {
            collection.CollectionChanged += (s, args) =>
            {
                if (args.Action == NotifyCollectionChangedAction.Add)
                {
                    AuftragsListe.ScrollIntoView(args.NewItems[0]);
                }
            };
        }
    }



    private void EingangsScanTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            if (eingangsScanTextBox.Text.inputCheckLieferschein())
            {
                EingangsLieferscheinModel lieferscheinScan = new EingangsLieferscheinModel(eingangsScanTextBox.Text);
                if (LieferscheinExistsCheck(lieferscheinScan) == true)
                {
                    MessageBox.Show("Lieferschein bereits gescannt. Datum aktualisiert");
                    sqlLieferschein.UpdateLieferschein(lieferscheinScan);
                    angezeigteLieferscheine.Add(lieferscheinScan);

                    UiCleanUp();
                }
                else
                {
                    sqlLieferschein.LieferscheinEingangsScan(lieferscheinScan);
                    angezeigteLieferscheine.Add(lieferscheinScan);
                    UiCleanUp();
                }

            }
            else
            {

                WrongInputAlarm(eingangsScanTextBox);
            }

        }
    }

    private void UiCleanUp()
    {
        eingangsScanTextBox.Background = Brushes.White;
        eingangsScanTextBox.Clear();
        AuftragsListe.ScrollIntoView(AuftragsListe.Items[AuftragsListe.Items.Count - 1]);
    }
    private bool LieferscheinExistsCheck(EingangsLieferscheinModel input)
    {
        bool output;
        try
        {
            sqlLieferschein.SucheNachLieferschein(input.Lieferschein);
            output = true;
        }
        catch
        {
            output = false;
        }
        return output;
    }
    private void KontrolleTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            if (KontrolleTextBox.Text.inputCheckLieferschein())
            {
                string searchResult = "Lieferschein nicht gefunden";
                

                try
                {
                    SearchLieferschein found = sqlLieferschein.SucheNachLieferschein(KontrolleTextBox.Text);
                    if (found.Lieferschein == KontrolleTextBox.Text)
                    {
                        searchResult = $"{found.Lieferschein} \nKommissionierung: {found.EingangsTS} \nMontage: {found.MontageTS}";
                        if(found.MitarbeiterId != null)
                        {
                            SqlMitarbeiter ma = new SqlMitarbeiter(GetConnectionString("PrivateMontageScan"));
                            var mitarbeiter = ma.GetMiarbeiterById(found.MitarbeiterId.ToString());
                            searchResult = searchResult + $" \nMitarbeiter: {mitarbeiter.Vorname} {mitarbeiter.Nachname}";
                        }
                    }
                }
                catch
                {

                }

                MessageBox.Show(searchResult);

                KontrolleTextBox.Clear();
                KontrolleTextBox.Background = Brushes.White;
            }
            else
            {
                WrongInputAlarm(KontrolleTextBox);
            }
        }

    }


    private void WrongInputAlarm(TextBox sender)
    {
        sender.Background = Brushes.Red;
        sender.Clear();
        sender.Focus();
    }

}