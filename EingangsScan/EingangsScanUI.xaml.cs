using MontageScanDataAccessLib;
using MontageScanLib;
using MontageScanLib.Models;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;




namespace EingangsScan;

public partial class EingangsScanUI : Window
{
    BindingList<AktiverLieferscheinModel> angezeigteLieferscheine = new BindingList<AktiverLieferscheinModel>();
    SqlLieferschein sqlLieferschein;
    public EingangsScanUI()
    {
        InitializeComponent();
        sqlLieferschein = new SqlLieferschein(GetConnectionString("PrivateMontageScan"));
        AuftragsListe.ItemsSource = angezeigteLieferscheine;
        //FillDisplayedList();
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
    private void FillDisplayedList()
    {
        List<AktiverLieferscheinModel> tempList = new List<AktiverLieferscheinModel>();
        tempList = sqlLieferschein.GetLast100RowsFromLieferschein();
        tempList.Reverse();
        foreach (var row in tempList)
        {
            angezeigteLieferscheine.Add(row);
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
                    MessageBoxResult result = MessageBox.Show("Lieferschein bereits vorhanden\nDatum aktualisieren?", "Confirmation", MessageBoxButton.YesNo);
                    if(result == MessageBoxResult.Yes)
                    {
                        sqlLieferschein.UpdateLieferschein(lieferscheinScan);
                        angezeigteLieferscheine.Add(new AktiverLieferscheinModel { Lieferschein = lieferscheinScan.Lieferschein, EingangsTS = lieferscheinScan.EingangsTS });
                    }
                    UiCleanUp();
                }
                else
                {
                    sqlLieferschein.LieferscheinEingangsScan(lieferscheinScan);
                    angezeigteLieferscheine.Add(new AktiverLieferscheinModel { Lieferschein = lieferscheinScan.Lieferschein, EingangsTS = lieferscheinScan.EingangsTS});
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
                        if (found.MitarbeiterId != null)
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