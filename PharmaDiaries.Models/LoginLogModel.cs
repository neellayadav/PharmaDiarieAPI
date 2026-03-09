using System;

namespace PharmaDiaries.Models
{
    public class LoginLogModel
    {
        public int LogID { get; set; }
        public int CompID { get; set; }
        public int UID { get; set; }
        public string? Source { get; set; }
        public DateTime LoginDT { get; set; }
        public DateTime? LogoutDT { get; set; }
    }

    public class LoginLogInsertRequest
    {
        public int CompID { get; set; }
        public int UID { get; set; }
        public string? Source { get; set; }
    }

    public class LoginLogUpdateRequest
    {
        public int LogID { get; set; }
    }

    public class LoginLogByMonthYearRequest
    {
        public int CompID { get; set; }
        public int UID { get; set; }
        public int MonthOf { get; set; }
        public int YearOf { get; set; }
    }

    public class LoginLogByDateRequest
    {
        public int CompID { get; set; }
        public int UID { get; set; }
        public DateTime DateOf { get; set; }
    }
}
