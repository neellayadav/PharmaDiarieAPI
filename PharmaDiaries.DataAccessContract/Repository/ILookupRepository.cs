using System;
using PharmaDiaries.Models;
using System.Collections.Generic;

namespace PharmaDiaries.DataAccessContract
{
	public interface ILookupRepository
	{
        List<Lookup> Lookuplist();

        List<Lookup> LookupListByType(String lookupType);

        bool DeleteLookUp(int compID, string code);

    }
}


