using System;
using PharmaDiaries.Models;
using System.Collections.Generic;

namespace PharmaDiaries.DataAccessContract
{
	public interface IAreasRepository
	{
        public List<AreasModel> AreaList(int compId);

        public List<RegionModel> RegionList(int compId);

        public List<HeadQuaterModel> HeadQuaterList(int compId);

        public List<HeadQuaterModel> HeadQuarterListByRegion(int compId, string region);

        public List<PatchModel> PatchListByHeadQuater(int compId, string hQuater);

        public bool Save(AreasModel areaModel);

        public bool Update(AreasModel areaModel);

        public bool Delete(int areaId, int compId, int modifiedBy);
    }
}


