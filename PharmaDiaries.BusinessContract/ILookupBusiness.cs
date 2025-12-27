using System;
using PharmaDiaries.Models;
using System.Collections.Generic;

namespace PharmaDiaries.BusinessContract
{
	public interface ILookupBusiness
	{
		List<Lookup> Lookuplist();

		List<Lookup> LookupListByType(String lookupType);

		bool DeleteLookUp(int compID, string code);

    }
}

