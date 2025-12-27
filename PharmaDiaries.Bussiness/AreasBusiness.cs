using System;
using System.Collections.Generic;
using PharmaDiaries.BusinessContract;
using PharmaDiaries.DataAccessContract;
using PharmaDiaries.Models;

namespace PharmaDiaries.Bussiness
{
	public class AreasBusiness : IAreasBusiness
	{

		private IAreasRepository _repository;

		public AreasBusiness(IAreasRepository repository)
		{
            _repository = repository;
		}

        public List<AreasModel> AreaList(int compId)
        {
            return _repository.AreaList(compId);
        }

        public List<HeadQuaterModel> HeadQuarterListByRegion(int compId, string region)
        {
            return _repository.HeadQuarterListByRegion(compId, region);
        }

        public List<PatchModel> PatchListByHeadQuater(int compId, string hQuater)
        {
            return _repository.PatchListByHeadQuater(compId, hQuater);
        }

        public List<RegionModel> RegionList(int compId)
        {
            return _repository.RegionList(compId);
        }

        public List<HeadQuaterModel> HeadQuaterList(int compId)
        {
            return _repository.HeadQuaterList(compId);
        }

        public bool Save(AreasModel areaModel)
        {
            return _repository.Save(areaModel);
        }

        public bool Update(AreasModel areaModel)
        {
            return _repository.Update(areaModel);
        }

        public bool Delete(int areaId, int compId, int modifiedBy)
        {
            return _repository.Delete(areaId, compId, modifiedBy);
        }
    }
}

