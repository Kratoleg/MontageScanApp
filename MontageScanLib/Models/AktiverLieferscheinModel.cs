using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MontageScanLib.Models
{
    public class AktiverLieferscheinModel
    {
        public int LieferscheinId { get; set; }

        public string Lieferschein { get; set; }
        public DateTime EingangsTS { get; set; }
        public bool? Storniert { get; set; }


        public string StringEingangsTS
        { get { return EingangsTS.ToString("dd.MM.yyyy HH:mm:ss"); } }
    }
}
