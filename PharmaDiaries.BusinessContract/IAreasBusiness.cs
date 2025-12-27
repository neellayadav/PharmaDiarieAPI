using System;
using PharmaDiaries.Models;
using System.Collections.Generic;

namespace PharmaDiaries.BusinessContract
{
	public interface IAreasBusiness
	{
        public List<AreasModel> AreaList(int compId);

		public List<RegionModel> RegionList(int compId);

        public List<HeadQuaterModel> HeadQuaterList(int compId);

        public List<HeadQuaterModel> HeadQuarterListByRegion(int compId, String region);

        public List<PatchModel> PatchListByHeadQuater(int compId, String hQuater);

        public bool Save(AreasModel areaModel);

        public bool Update(AreasModel areaModel);

        public bool Delete(int areaId, int compId, int modifiedBy);

    }
}

