using System;
using System.Collections.Generic;
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


namespace MonteuerScan;

/// <summary>
/// Interaction logic for MonteuerScanUI.xaml
/// </summary>
public partial class MonteuerScanUI : Window
{
    private MitarbeiterModel _loggedInMitarbeiter;
    private SqlMitarbeiter _sqlMa;

    private SqlLieferschein _sqlLs;
    private SearchLieferschein _lieferscheinInput;
    BindingList<DisplayedModel> angezeigteLieferscheine;
    public MonteuerScanUI(string connectionString)
    {

        _sqlMa = new SqlMitarbeiter(connectionString);
        _sqlLs = new SqlLieferschein(connectionString);
        angezeigteLieferscheine = new BindingList<DisplayedModel>();
        InitializeComponent();

        DisplayName.Text = "Kein Mitarbeiter";
        AuftragsListe.ItemsSource = angezeigteLieferscheine;

    }
    public MonteuerScanUI()
    {
        _sqlMa = new SqlMitarbeiter(GetConnectionString("RV24_Montage"));
        _sqlLs = new SqlLieferschein(GetConnectionString("RV24_Montage"));
        angezeigteLieferscheine = new BindingList<DisplayedModel>();
        InitializeComponent();
        DisplayName.Text = "Kein Mitarbeiter";
        AuftragsListe.ItemsSource = angezeigteLieferscheine;
    }


    private string GetConnectionString(string name)
    {
        ConnectionStringSettings? settings =
            ConfigurationManager.ConnectionStrings[name];
        return settings?.ConnectionString;
    }



    private void saveLieferscheinToDb(string lieferschein)
    {
        SearchLieferschein wanted = new SearchLieferschein();

        //Lieferschein nicht gefunden
        wanted = _sqlLs.SucheNachLieferschein(lieferschein);

        if (wanted == null)
        {
            wanted = new SearchLieferschein { Lieferschein = lieferschein};
            _sqlLs.LieferscheinEingangsScan(wanted);
            wanted = _sqlLs.SucheNachLieferschein(wanted);

        }

        if (wanted.MontageTS.Year > 2000)
            {
                MessageBox.Show($"Lieferschein: {wanted.Lieferschein} wurde bereits montiert!\n\n Dowód dostawy został już przetworzony");
            }
            else if (wanted.Storniert == 1)
            {
                MessageBox.Show("Stop! Lieferschein wurde storniert!\n\nZamówienie zostało anulowane! Nie montuj!");
            }
            else if (wanted.MontageTS.Year < 2000 && wanted.Storniert == 0)
            {
                wanted.MontageTS = DateTime.Now;
                wanted.MitarbeiterId = _loggedInMitarbeiter.MitarbeiterId;
                _sqlLs.LieferscheinMontageScan(wanted);
                saveLieferscheinToDisplayList(wanted);
            }

    }

    private void saveLieferscheinToDisplayList(SearchLieferschein input)
    {
        angezeigteLieferscheine.Add(new DisplayedModel { Lieferschein = input.Lieferschein, Nachname = _loggedInMitarbeiter.Nachname, TimeStamp = input.MontageTS });
    }

    private void mitarbeiterLogin(string mitarbeiterChipId)
    {
        if (_loggedInMitarbeiter == null)
        {
            _loggedInMitarbeiter = _sqlMa.GetMiarbeiterByChip(mitarbeiterChipId);

        }
        else if (_loggedInMitarbeiter != null && _loggedInMitarbeiter.ChipId == mitarbeiterChipId)
        {
            _loggedInMitarbeiter = null;

        }
        else if (_loggedInMitarbeiter != null && _loggedInMitarbeiter.ChipId != mitarbeiterChipId)
        {
            _loggedInMitarbeiter = _sqlMa.GetMiarbeiterByChip(mitarbeiterChipId);
        }
        displayMitarbeiterName(_loggedInMitarbeiter);
    }

    private void displayMitarbeiterName(MitarbeiterModel input)
    {
        if (input != null)
        {
            DisplayName.Text = $"{input.Vorname} {input.Nachname}";
        }
        else
        {
            DisplayName.Text = "Kein Mitarbeiter";
        }
    }

    private void inputTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            if (inputTextBox.Text.inputCheckLieferschein())
            {
                if (_loggedInMitarbeiter == null)
                {
                    InvalidInputWarning();
                }
                else if (_loggedInMitarbeiter != null)
                {
                    saveLieferscheinToDb(inputTextBox.Text);

                    ValidinputWarning();
                }
            }
            else if (inputTextBox.Text.inputCheckChipId())
            {
                mitarbeiterLogin(inputTextBox.Text);
                ValidinputWarning();
            }
            else
            {
                InvalidInputWarning();
            }
        }



    }
    private void InvalidInputWarning()
    {
        inputTextBox.Clear();
        inputTextBox.Background = Brushes.Red;
        inputTextBox.Focus();
    }
    private void ValidinputWarning()
    {
        inputTextBox.Clear();
        inputTextBox.Background = Brushes.White;
        inputTextBox.Focus();
        DisplayName.Background = Brushes.White;
    }

}
