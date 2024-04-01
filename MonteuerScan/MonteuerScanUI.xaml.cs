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
    private MontageLieferscheinModel _lieferscheinInput;
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
        _sqlMa = new SqlMitarbeiter(GetConnectionString("PrivateMontageScan"));
        _sqlLs = new SqlLieferschein(GetConnectionString("PrivateMontageScan"));
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
        _lieferscheinInput = new MontageLieferscheinModel(lieferschein);
        try
        {
            _sqlLs.SucheNachLieferschein(lieferschein);
            _sqlLs.LieferscheinMontageScan(_lieferscheinInput, _loggedInMitarbeiter.MitarbeiterId);
        }
        catch
        {
            EingangsLieferscheinModel input = new EingangsLieferscheinModel(lieferschein);
            _sqlLs.LieferscheinEingangsScan(input);
            _sqlLs.LieferscheinMontageScan(_lieferscheinInput, _loggedInMitarbeiter.MitarbeiterId);
        }
    }
    private void saveLieferscheinToDisplayList()
    {
        angezeigteLieferscheine.Add(new DisplayedModel { Lieferschein = _lieferscheinInput.Lieferschein, Nachname = _loggedInMitarbeiter.Nachname, TimeStamp = _lieferscheinInput.MontageTS });
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
                    saveLieferscheinToDisplayList();
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
