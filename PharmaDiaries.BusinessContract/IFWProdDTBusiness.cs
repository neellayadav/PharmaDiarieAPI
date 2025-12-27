using PharmaDiaries.Models;
using System.Collections.Generic;

namespace PharmaDiaries.BusinessContract
{
    public interface IFWProdDTBusiness
    {
        List<FWProdDT4Report> GetAllFieldworkProd(string transId);

        bool DeleteProdDT(string transID, int sno, string uid);

        bool Save(FieldworkProdDT prod);
    }


}