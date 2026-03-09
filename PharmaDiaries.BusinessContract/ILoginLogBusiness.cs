using System;
using System.Collections.Generic;
using PharmaDiaries.Models;

namespace PharmaDiaries.BusinessContract
{
    public interface ILoginLogBusiness
    {
        LoginLogModel? Save(LoginLogInsertRequest request);
        LoginLogModel? Update(LoginLogUpdateRequest request);
        List<LoginLogModel> GetByCompUID(int compId, int uid);
        LoginLogModel? GetByLogId(int logId);
        List<LoginLogModel> GetByCompUIDMonthYear(int compId, int uid, int monthOf, int yearOf);
        List<LoginLogModel> GetByCompUIDDate(int compId, int uid, DateTime dateOf);
        List<LoginLogModel> GetByCompIdMonthYear(int compId, int monthOf, int yearOf);
        List<LoginLogModel> GetByCompIdDate(int compId, DateTime dateOf);
    }
}
