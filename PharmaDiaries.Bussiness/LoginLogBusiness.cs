using System;
using System.Collections.Generic;
using PharmaDiaries.BusinessContract;
using PharmaDiaries.DataAccessContract.Repository;
using PharmaDiaries.Models;

namespace PharmaDiaries.Bussiness
{
    public class LoginLogBusiness : ILoginLogBusiness
    {
        private readonly ILoginLogRepository _repository;

        public LoginLogBusiness(ILoginLogRepository repository)
        {
            _repository = repository;
        }

        public LoginLogModel? Save(LoginLogInsertRequest request)
        {
            return _repository.Save(request);
        }

        public LoginLogModel? Update(LoginLogUpdateRequest request)
        {
            return _repository.Update(request);
        }

        public List<LoginLogModel> GetByCompUID(int compId, int uid)
        {
            return _repository.GetByCompUID(compId, uid);
        }

        public LoginLogModel? GetByLogId(int logId)
        {
            return _repository.GetByLogId(logId);
        }

        public List<LoginLogModel> GetByCompUIDMonthYear(int compId, int uid, int monthOf, int yearOf)
        {
            return _repository.GetByCompUIDMonthYear(compId, uid, monthOf, yearOf);
        }

        public List<LoginLogModel> GetByCompUIDDate(int compId, int uid, DateTime dateOf)
        {
            return _repository.GetByCompUIDDate(compId, uid, dateOf);
        }

        public List<LoginLogModel> GetByCompIdMonthYear(int compId, int monthOf, int yearOf)
        {
            return _repository.GetByCompIdMonthYear(compId, monthOf, yearOf);
        }

        public List<LoginLogModel> GetByCompIdDate(int compId, DateTime dateOf)
        {
            return _repository.GetByCompIdDate(compId, dateOf);
        }
    }
}
