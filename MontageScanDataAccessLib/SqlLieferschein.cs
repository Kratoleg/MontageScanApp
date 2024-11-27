using MontageScanLib.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics.Tracing;
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

    public void LieferscheinEingangsScan(string input)
    {
        SearchLieferschein output = new SearchLieferschein { Lieferschein = input };
        LieferscheinEingangsScan(output);

    }
    public void LieferscheinEingangsScan(SearchLieferschein input)
    {
            input.EingangsTS = DateTime.Now;
            input.Storniert = 0;
            string command = "insert into dbo.Lieferschein (Lieferschein, EingangsTS, Storniert) values (@lieferschein,  @eingangsTs, @storno);";
            string lieferschein = input.Lieferschein;
            DateTime eingangsTs = input.EingangsTS;
            int storno = input.Storniert;
            dbAccess.SaveData(command, new { lieferschein, eingangsTs, storno }, _connectionString);
    }


    public List<AktiverLieferscheinModel> GetLast100RowsFromLieferschein()
    {
        string command = "select top 100 LieferscheinId, Lieferschein, EingangsTS, Storniert from dbo.Lieferschein order by EingangsTS DESC;";
        return dbAccess.LoadData<AktiverLieferscheinModel, dynamic>(command, new { }, _connectionString);
    }

    public List<AktiverLieferscheinModel> GetOffeneMontageAuftraege()
    {
        //PRE-Release: SQL statement muss noch geprüft werden
        string command = "select ls.lieferscheinId, ls.lieferschein, ls.EingangsTS, ls.Storniert from dbo.Lieferschein ls where ls.Storniert is NULL or ls.Storniert = 0 AND NOT EXISTS(SELECT 1 from Montage m where m.LieferscheinId = ls.LieferscheinId);";
        return dbAccess.LoadData<AktiverLieferscheinModel, dynamic>(command, new { }, _connectionString);
    }


    private void GetLieferscheinId(SearchLieferschein input)
    {
        string command = "select LieferscheinId from dbo.Lieferschein where Lieferschein = @lieferschein;";
        string lieferschein = input.Lieferschein;
        input.LieferscheinId = dbAccess.LoadData<int, dynamic>(command, new { lieferschein }, _connectionString).FirstOrDefault();
    }

    public void LieferscheinMontageScan(SearchLieferschein input)
    {
        //Prüfen ob Auftrag storniert ist!!
        if (input.LieferscheinId < 1)
        {
            LieferscheinEingangsScan(input);
            GetLieferscheinId(input);
        }
        
        if (input.LieferscheinId != null)
        {
            int mitarbeiterId = input.MitarbeiterId;
            int lieferscheinId = input.LieferscheinId;
            DateTime montageTs = input.MontageTS;
            int storniert = input.Storniert;

            string command = "insert into dbo.Montage (MitarbeiterId, LieferscheinId, MontageTS) values (@mitarbeiterId, @lieferscheinId, @montageTs);";
            dbAccess.SaveData(command, new { mitarbeiterId, lieferscheinId, montageTs }, _connectionString);
        }

    }



    public SearchLieferschein SucheNachLieferschein(SearchLieferschein input)
    {

        string command = "select ls.lieferscheinId, ls.lieferschein, ls.EingangsTS, m.MontageTS, ma.MitarbeiterId, ls.Storniert from Montage m inner join dbo.Mitarbeiter ma on ma.MitarbeiterId=m.MitarbeiterId right join dbo.Lieferschein ls on ls.LieferscheinId = m.LieferscheinId where ls.lieferschein = @search;";
        string search = input.Lieferschein;
        input = dbAccess.LoadData<SearchLieferschein, dynamic>(command, new { search }, _connectionString).FirstOrDefault();

        try
        {
            IsValidTimeStamp(input.EingangsTS);
          
            

        }
        catch 
        {

            throw new Exception("Lieferschein nicht gefunden");
        }
        return input;
     

    }
    public SearchLieferschein SucheNachLieferschein(string stringInput)
    {
        SearchLieferschein input = new SearchLieferschein{Lieferschein = stringInput };

        string command = "select ls.lieferscheinId, ls.lieferschein, ls.EingangsTS, m.MontageTS, ma.MitarbeiterId, ls.Storniert from Montage m inner join dbo.Mitarbeiter ma on ma.MitarbeiterId=m.MitarbeiterId right join dbo.Lieferschein ls on ls.LieferscheinId = m.LieferscheinId where ls.lieferschein = @search ;";
        string search = input.Lieferschein;

        input = dbAccess.LoadData<SearchLieferschein, dynamic>(command, new { search }, _connectionString).FirstOrDefault();
        
        if(input.EingangsTS == null)
        {
            throw new Exception("Lieferschein nicht gefunden");
        }
        else if(IsValidTimeStamp(input.EingangsTS))
        { return input; }
        throw new Exception("Lieferschein nicht gefunden");
    }



    public void UpdateLsStornoStatus(SearchLieferschein input)
    {
        string command = "update dbo.Lieferschein set Storniert = @storno where Lieferschein = @search;";
        int storno = input.Storniert;
        string search = input.Lieferschein;
        dbAccess.SaveData(command, new { search, storno }, _connectionString);
    }




    private bool IsValidTimeStamp(DateTime input)
    {
        bool output = false;
        int rightNow = DateTime.Now.Year;
        int inputTS = input.Year;
        if(inputTS >= (rightNow - 5))
        {
            output = true;
        }
        return output;
    }

    public SearchLieferschein TemporäreFunktionDieAllesZumLieferscheinHolt(SearchLieferschein input)
    {  
        string command = "select ls.lieferscheinId, ls.lieferschein, ls.EingangsTS, m.MontageTS, ma.MitarbeiterId, ls.Storniert from Montage m inner join dbo.Mitarbeiter ma on ma.MitarbeiterId=m.MitarbeiterId right join dbo.Lieferschein ls on ls.LieferscheinId = m.LieferscheinId where ls.lieferschein = @search;";
        string search = input.Lieferschein;
        input = dbAccess.LoadData<SearchLieferschein, dynamic>(command, new { search }, _connectionString).FirstOrDefault();
        return input;
    }

}
