﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MontageScanLib.Models;

public class SearchLieferschein
{
    public int? LieferscheinId { get; set; }
    public string? Lieferschein { get; set; }
    public DateTime? EingangsTS { get; set; }
    public DateTime? MontageTS { get; set; }
    public int? MitarbeiterId { get; set; }
}
