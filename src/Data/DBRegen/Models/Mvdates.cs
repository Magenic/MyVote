using System;
using System.Collections.Generic;

namespace DBRegen.Models
{
    public partial class Mvdates
    {
        public int DateKey { get; set; }
        public DateTime DateId { get; set; }
        public string DayNme { get; set; }
        public string DayShort { get; set; }
        public byte DayNod { get; set; }
        public short CalendarYear { get; set; }
        public short CalendarYearNod { get; set; }
        public int CalendarQuarter { get; set; }
        public string CalendarQuarterName { get; set; }
        public string CalendarQuarterShort { get; set; }
        public byte CalendarQuarterNod { get; set; }
        public int CalendarMonth { get; set; }
        public string CalendarMonthName { get; set; }
        public string CalendarMonthShort { get; set; }
        public byte CalendarMonthNod { get; set; }
        public int CalendarWeek { get; set; }
        public string CalendarWeekName { get; set; }
        public string CalendarWeekShort { get; set; }
        public byte CalendarWeekNod { get; set; }
    }
}
