using MontageScanLib.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MontageScanDataAccessLib;

public class SqlLieferschein
{
    private readonly string _connectionString;
    private SqlAccess dbAccess = new SqlAccess();

    public SqlLieferschein(string connectionstring)
    {
        _connectionString = connectionstring;
    }

    public void LieferscheinEingangsScan(EingangsLieferscheinModel input)
    {
        if (InputLsCheck(input) == true)
        {
            string command = "insert into dbo.Lieferschein (Lieferschein, EingangsTS) values (@Lieferschein,  @EingangsTS);";

            dbAccess.SaveData(command,
        new { input.Lieferschein, input.EingangsTS },
        _connectionString);
        }
        else
        {
            throw new Exception("no valid input");
        }

    }
    public void UpdateLieferschein(EingangsLieferscheinModel input)
    {
        if (InputLsCheck(input) == true)
        {
            string command = "update dbo.Lieferschein set EingangsTS = @EingangsTS where Lieferschein = @Lieferschein ;";

            dbAccess.SaveData(command,
        new { input.Lieferschein, input.EingangsTS },
        _connectionString);
        }
        else
        {
            throw new Exception("no valid input");
        }
    }


    private int GetLieferscheinId(string lieferschein)
    {
        string command = "select LieferscheinId from dbo.Lieferschein where Lieferschein = @lieferschein;";
        try
        {

            int lieferscheinID = dbAccess.LoadData<int, dynamic>(command, new { lieferschein }, _connectionString).FirstOrDefault();
            return lieferscheinID;
        }
        catch (Exception)
        {

            return 0;
        }

    }
    public void LieferscheinMontageScan(MontageLieferscheinModel input, int MonteurId)
    {
        //Get ID from dbo.Lieferschein. If not exists insert and then create new row in dbo.Montage with needed stuff
        int lieferscheinId = GetLieferscheinId(input.Lieferschein);


        if (lieferscheinId != null && lieferscheinId > 0) //LieferscheinID gefunden
        {
            WriteToMontage(MonteurId, lieferscheinId, input.MontageTS);
        }
        else
        {
            //Write Lieferschein to Lieferschein Db, get ID und write to Montage
            EingangsLieferscheinModel temp = new EingangsLieferscheinModel(input.Lieferschein);
            LieferscheinEingangsScan(temp);
            int id = GetLieferscheinId(temp.Lieferschein);
            WriteToMontage(MonteurId, id, input.MontageTS);
        }
    }

    private void WriteToMontage(int mitarbeiterId, int lieferscheinId, DateTime montageTs)
    {
        string command = "insert into dbo.Montage (MitarbeiterId, LieferscheinId, MontageTS) values (@mitarbeiterId, @lieferscheinId, @montageTs);";
        dbAccess.SaveData(command, new { mitarbeiterId, lieferscheinId, montageTs }, _connectionString);
    }
    

    //This Method muss nun völlig überarbeitet werden. 
    public SearchLieferschein SucheNachLieferschein(string lieferschein)
    {
        SearchLieferschein? output = new();
        string command = "select ls.lieferscheinId, ls.lieferschein, ls.EingangsTS, m.MontageTS, ma.MitarbeiterId from Montage m inner join dbo.Mitarbeiter ma on ma.MitarbeiterId=m.MitarbeiterId inner join dbo.Lieferschein ls on ls.LieferscheinId = m.LieferscheinId where ls.lieferschein = @lieferschein;";

        output = dbAccess.LoadData<SearchLieferschein, dynamic>(command, new { lieferschein }, _connectionString).FirstOrDefault();

        if (output != null)
        {
            return output;
        }
        else { throw new Exception("Lieferschein nicht gefunden"); }
        
    }

    private bool InputLsCheck(EingangsLieferscheinModel input)
    {
        bool output = false;
        if (input.Lieferschein.Length == 7 && (input.EingangsTS.AddYears(-1).Year == DateTime.Now.Year || input.EingangsTS.Year == DateTime.Now.Year))
        {
            output = true;
        }
        return output;
    }
}
