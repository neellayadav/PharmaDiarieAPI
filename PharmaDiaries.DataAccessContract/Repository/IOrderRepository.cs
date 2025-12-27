using PharmaDiaries.Models;

namespace PharmaDiaries.DataAccessContract.Repository
{
    public interface IOrderRepository
    {
        bool Save(OrderModel order);
        List<OrderModel> GetByTransID(string transId);
    }
}
