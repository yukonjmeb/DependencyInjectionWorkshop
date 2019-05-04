namespace DependencyInjectionWorkshop.Models
{
    using System.Net.Http;

    public interface IFailedCounter
    {
        void Add(string accountId);

        int Get(string accountId);

        void Reset(string accountId);

        void CheckAccountIsLocked(string accountId);
    }
}