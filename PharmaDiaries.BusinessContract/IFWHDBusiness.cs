using PharmaDiaries.Models;
using System.Collections.Generic;

namespace PharmaDiaries.BusinessContract
{
	public interface IFWHDBusiness
	{
		List<FieldWorkHeader> FWHeaderList();

		bool Save(FieldWorkHeader fwSave);

		bool Delete(int compId, string transid, string userid);

		bool OtherworkSave(FieldworkOthers owSave);

		List<FieldWorkHeader> GetEmpDateWiseFW(int compId, int uid, String empWorkDate, String periodOf);

		FWSummary GetFieldworkSummary(int compId, int uid);

		List<FWHeader4Report> GetFWMonthlyReport(int compId, int monthOf, int yearOf);

    }
}
