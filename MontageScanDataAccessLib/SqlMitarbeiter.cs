using MontageScanLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MontageScanDataAccessLib;

public class SqlMitarbeiter
{
    private readonly string _connectionString;
    private SqlAccess dbAccess = new SqlAccess();

    public SqlMitarbeiter(string connectionstring)
    {
        _connectionString = connectionstring;
    }


    public void AddMiarbeiter(MitarbeiterModel mitarbeiter)
    {
        string command = "insert into dbo.Mitarbeiter (Vorname, Nachname, ChipId) values (@Vorname, @Nachname, @ChipId);";
        dbAccess.SaveData(command,
    new { mitarbeiter.Vorname, mitarbeiter.Nachname, mitarbeiter.ChipId },
    _connectionString);
    }

    public void UpdateMitarbeiterNameByChipId(MitarbeiterModel mitarbeiter)
    {
        string command = "update dbo.Mitarbeiter set Vorname = @Vorname, Nachname = @Nachname where ChipId = @ChipId;";
        dbAccess.SaveData(command, mitarbeiter, _connectionString);
    }
    public MitarbeiterModel GetMiarbeiterByChip(string ChipId)
    {
        string command = "select MitarbeiterId, Vorname, Nachname, ChipId from dbo.Mitarbeiter where ChipId = @ChipId;";
        MitarbeiterModel output = dbAccess.LoadData<MitarbeiterModel, dynamic>(command, new { ChipId }, _connectionString).FirstOrDefault();

        if (checkForValidModel(output) == true)
        {
            return output;
        }
        else
        {
            return null;
        }

    }

    private bool checkForValidModel(MitarbeiterModel valueToCheck)
    {
        bool output = false;

        if (valueToCheck != null)
        {
            if (valueToCheck.Vorname != null && valueToCheck.Nachname != null)
            {
                if (valueToCheck.Vorname.Length > 0 && valueToCheck.Nachname.Length > 0)
                {
                    output = true;
                }

            }
        }

        return output;
    }

    public MitarbeiterModel GetMiarbeiterById(string mitarbeiterId)
    {
        string command = "select MitarbeiterId, Vorname, Nachname, ChipId from dbo.Mitarbeiter where MitarbeiterId = @mitarbeiterId;";
        MitarbeiterModel output = dbAccess.LoadData<MitarbeiterModel, dynamic>(command, new { mitarbeiterId }, _connectionString).FirstOrDefault();

        if (checkForValidModel(output) == true)
        {
            return output;
        }
        else
        {
            return null;
        }

    }
}
