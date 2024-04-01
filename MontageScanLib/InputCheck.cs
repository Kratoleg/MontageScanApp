using MontageScanLib.Models;
namespace MontageScanLib;

public static class InputCheck
{

    /// <summary>
    /// Hardcoded Prüfung der eingabe
    /// </summary>
    /// <param name="input">Inputstring</param>
    /// <returns>true wenn eingabe Buchstabe und 6 Ziffern sind</returns>
    public static bool inputCheckLieferschein(this string input)
    {
        bool output = false;

        if (input != null && input.Length == 7)
        {
            if (char.IsLetter(input[0]))
            {
                for (int i = 1; i < input.Length; i++)
                {
                    if (char.IsDigit(input[i]))
                    {
                        output = true;
                    }
                    else
                    {
                        output = false;
                    }
                }

            }
            else
            {
                output = false;
            }
        }
        else
        {
            output = false;
        }
        return output;
    }


    /// <summary>
    /// Checks if the Input could be a valid ChipId
    /// </summary>
    /// <param name="input"> input chipid</param>
    /// <returns>true if chipId >5 && <15 and every char is a number</returns>
    public static bool inputCheckChipId(this string input)
    {

        if(input.Length > 5 && input.Length < 15)
        {
            bool output = false;
            foreach (char number in input)
            {
                if (char.IsNumber(number))
                {
                    output = true;
                }
                else
                {
                    return false;
                }
            }
            return output;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Checks if MitarbeiterModel has values in vorname and nachname
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool validMitarbeiterInput(this MitarbeiterModel input)
    {
        bool output = false;

        if (input.Vorname.Length > 0 && input.Vorname.Length < 20 && input.Vorname != null &&
            input.Nachname.Length > 0 && input.Nachname.Length < 20 && input.Nachname != null
            )
        {
            output = true;
        }
        return output;
    }
}
