using PharmaDiaries.Models;
using System.Collections.Generic;

namespace PharmaDiaries.DataAccessContract
{
    public interface IFWEmpDTRepository
    {
        List<FWEmpDT4Report> GetAllFieldworkEmp(string transId);

        bool DeleteEmpDT(string transID, int SNo, string uid);

        bool Save(FieldworkEmpDT fwEmpDT);

    }
}