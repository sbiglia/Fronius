using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Animation;

namespace MySolarViewer
{
    public class SolarDataLog
    {
        public DateTime TimeStamp { get; set; }
        public decimal Eday { get; set; }
        public decimal EYear { get; set; }
        public decimal ETotal { get; set; }
        public string Mode { get; set; }
        public int  PAkku { get; set; }
        public decimal PGrid { get; set; }
        public decimal PLoad { get; set; }
        public decimal Ppv { get; set; }
        public decimal RelAutonomy { get; set; }
        public decimal RelSelfConsumption { get; set; }
        public int BackupMode { get; set; }
        public int BatteryStandBy { get; set; }
    }
}
