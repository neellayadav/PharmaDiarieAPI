using PharmaDiaries.Models;
using System.Collections.Generic;

namespace PharmaDiaries.DataAccessContract
{
    public interface IFWProdDTRepository
    {
        List<FWProdDT4Report> GetAllFieldworkProd(string transId);

        bool DeleteProdDT(string transID, int sno, string uid);

        bool Save(FieldworkProdDT prod);

    }
}