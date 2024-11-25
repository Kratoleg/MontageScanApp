using MontageScanLib.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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

    public void LieferscheinEingangsScan(SearchLieferschein input)
    {
        if (InputLsCheck(input) == true)
        {
            input.EingangsTS = DateTime.Now;
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
    //public void UpdateLieferschein(EingangsLieferscheinModel input)
    //{
    //    if (InputLsCheck(input) == true)
    //    {
    //        string command = "update dbo.Lieferschein set EingangsTS = @EingangsTS where Lieferschein = @Lieferschein ;";

    //        dbAccess.SaveData(command,
    //    new { input.Lieferschein, input.EingangsTS },
    //    _connectionString);
    //    }
    //    else
    //    {
    //        throw new Exception("no valid input");
    //    }
    //}

    public List<AktiverLieferscheinModel> GetLast100RowsFromLieferschein()
    {
        string command = "select top 100 LieferscheinId, Lieferschein, EingangsTS, Storniert from dbo.Lieferschein order by EingangsTS DESC;";
        return dbAccess.LoadData<AktiverLieferscheinModel, dynamic>(command, new { }, _connectionString);
    }
    public List<AktiverLieferscheinModel> GetOffeneMontageAuftraege()
    {
        //PRE-Release: SQL statement muss noch geprüft werden
        string command = "";
        return dbAccess.LoadData<AktiverLieferscheinModel, dynamic>(command, new { }, _connectionString);
    }


    private void GetLieferscheinId(SearchLieferschein input)
    {
        string command = "select LieferscheinId from dbo.Lieferschein where Lieferschein = @lieferschein;";
        input.LieferscheinId = dbAccess.LoadData<int, dynamic>(command, new { input.Lieferschein }, _connectionString).FirstOrDefault();
    }

    public void LieferscheinMontageScan(SearchLieferschein input)
    {
        if(input.LieferscheinId == null && InputLsCheck(input))
        {
            LieferscheinEingangsScan(input);
            GetLieferscheinId(input);
        }
        string command = "insert into dbo.Montage (MitarbeiterId, LieferscheinId, MontageTS) values (@MitarbeiterId, @LieferscheinId, @MontageTS);";
        dbAccess.SaveData(command, new { input.MitarbeiterId, input.LieferscheinId, input.MontageTS }, _connectionString);
    }



    public void SucheNachLieferschein(SearchLieferschein input)
    {

        string command = "select ls.lieferscheinId, ls.lieferschein, ls.EingangsTS, m.MontageTS, ma.MitarbeiterId ls.from Montage m inner join dbo.Mitarbeiter ma on ma.MitarbeiterId=m.MitarbeiterId right join dbo.Lieferschein ls on ls.LieferscheinId = m.LieferscheinId where ls.lieferschein = @lieferschein;";

        input = dbAccess.LoadData<SearchLieferschein, dynamic>(command, new { input.Lieferschein }, _connectionString).FirstOrDefault();

        if (input.EingangsTS != null)
        {
            return;
        }
        else { throw new Exception("Lieferschein nicht gefunden"); }

    }

    private bool InputLsCheck(SearchLieferschein input)
    {
        bool output = false;
        if (input.Lieferschein.Length == 7 && (input.EingangsTS.AddYears(-1).Year == DateTime.Now.Year || input.EingangsTS.Year == DateTime.Now.Year))
        {
            output = true;
        }
        return output;
    }

    public void UpdateLsStornoStatus(SearchLieferschein input)
    {
        string command = "update dbo.Lieferschein set Storniert = @Storniert where Lieferschein = @Lieferschein ;";
        dbAccess.SaveData(command, new { input.Lieferschein, input.Storniert }, _connectionString);

    }


    public SearchLieferschein TemporäreFunktionDieAllesZumLieferscheinHolt(string liferschein)
    {
        string command = "select ls.lieferscheinId, ls.lieferschein, ls.EingangsTS, m.MontageTS, ma.MitarbeiterId ls.Storniert from Montage m inner join dbo.Mitarbeiter ma on ma.MitarbeiterId=m.MitarbeiterId right join dbo.Lieferschein ls on ls.LieferscheinId = m.LieferscheinId where ls.lieferschein = @liferschein;";

        return dbAccess.LoadData<SearchLieferschein, dynamic>(command, new { liferschein }, _connectionString).FirstOrDefault();

    }




}
