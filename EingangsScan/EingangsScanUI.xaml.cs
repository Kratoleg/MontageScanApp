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
    BindingList<AktiverLieferscheinModel> offeneMontageListe = new BindingList<AktiverLieferscheinModel>();
    SqlLieferschein sqlLieferschein;
    public EingangsScanUI()
    {
        InitializeComponent();
        sqlLieferschein = new SqlLieferschein(GetConnectionString("RV24_Montage"));
        AuftragsListe.ItemsSource = angezeigteLieferscheine;
        OffeneMontageListe.ItemsSource = offeneMontageListe;
        RefreshMontageListe();
        FillDisplayedList();

    }

    private string GetConnectionString(string name)
    {
        ConnectionStringSettings? settings =
            ConfigurationManager.ConnectionStrings[name];
        return settings?.ConnectionString;
    }


    private void RefreshMontageListe()
    {
        List<AktiverLieferscheinModel> tempList = new List<AktiverLieferscheinModel>();
        tempList = sqlLieferschein.GetOffeneMontageAuftraege();
        foreach (var row in tempList)
        {
            offeneMontageListe.Add(row);
        }
    }
    private void RefreshMontageListe(object sender, RoutedEventArgs e)
    {
        offeneMontageListe.Clear();
        List<AktiverLieferscheinModel> tempList = new List<AktiverLieferscheinModel>();
        tempList = sqlLieferschein.GetOffeneMontageAuftraege();
        foreach (var row in tempList)
        {
            offeneMontageListe.Add(row);
        }

        //RefreshButton.Visibility = Visibility.Hidden;

        //Task.Run(async () =>
        //{
        //    await Task.Delay(5000);
        //    RefreshButton.Visibility = Visibility.Visible;
        //});


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
                //Prüfen ob Lieferschein hinterlegt ist und ob er gestopt ist 

                if (LieferscheinExistsCheck(eingangsScanTextBox.Text) == true)
                {
                    //Lieferschein Existiert. Soll er gestoppt werden oder nicht?
                    SearchLieferschein wanted = sqlLieferschein.SucheNachLieferschein(eingangsScanTextBox.Text);
                    if (wanted.MontageTS.Year < 2000)
                    {

                        EingangsMessageBox option = EingangsMessageBox.fehler;

                        if (wanted.Storniert == 1)
                        {//LS ist gestopt
                            option = EingangsMessageBox.reaktivieren;

                        }
                        else if (wanted.Storniert == 0)
                        {//LS nicht gestopt
                            option = EingangsMessageBox.stornieren;


                        }

                        EingangsScanMessageBox popUp = new EingangsScanMessageBox(option);
                        popUp.ShowDialog();
                        if (popUp.DialogResult == true)
                        {//Positiv, Swtich Stornostatus
                            wanted.Storniert = 1 - wanted.Storniert;
                            sqlLieferschein.UpdateLsStornoStatus(wanted);
                        }
                    }
                    else if (wanted.MontageTS.Year > 2000)
                    {
                        MessageBox.Show("Lieferschein bereits montiert\nEr kann nicht mehr gestoppt werden");
                    }
                    UiCleanUp();
                }
                else
                {
                    //Lieferschein nicht gefunden. Wird neu angelegt.
                    sqlLieferschein.LieferscheinEingangsScan(eingangsScanTextBox.Text);
                    angezeigteLieferscheine.Add(new AktiverLieferscheinModel { Lieferschein = eingangsScanTextBox.Text, EingangsTS = DateTime.Now });
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

    private bool LieferscheinExistsCheck(string input)
    {
       SearchLieferschein wanted =  sqlLieferschein.SucheNachLieferschein(input);
        if (wanted.EingangsTS.Year > 2000)
        {
            return true;
        }
        else return false;
    }


    private void KontrolleTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            if (KontrolleTextBox.Text.inputCheckLieferschein())
            {
                SearchLieferschein found = new SearchLieferschein { Lieferschein = KontrolleTextBox.Text };
                string searchResult = "Lieferschein nicht gefunden";
                try
                {

                    found = sqlLieferschein.SucheNachLieferschein(found);
                    if (found.EingangsTS.Year > 2000)
                    {
                        searchResult = $"{found.Lieferschein} \nKommissionierung: {found.EingangsTS}";
                        if (found.MitarbeiterId != null && found.MitarbeiterId > 0)
                        {
                            SqlMitarbeiter ma = new SqlMitarbeiter(GetConnectionString("PrivateMontageScan"));
                            var mitarbeiter = ma.GetMiarbeiterById(found.MitarbeiterId.ToString());
                            searchResult = searchResult + $" \nMontage Datum: {found.MontageTS}\nMonteur: {mitarbeiter.Vorname} {mitarbeiter.Nachname}";
                        }
                    }
                    if (found.Storniert == 1)
                    {
                        searchResult = searchResult + $"\nBearbeitungsstatus: Auftrag gestoppt!";
                    }
                    if (found.Storniert == 0 || found.Storniert == null)
                    {
                        searchResult = searchResult + $"\nBearbeitungsstatus: Auftrag in bearbeitung!";
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