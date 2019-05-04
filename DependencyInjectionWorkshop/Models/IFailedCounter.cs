namespace DependencyInjectionWorkshop.Models
{
    using System.Net.Http;

    public interface IFailedCounter
    {
        HttpResponseMessage Add(string accountId);

        HttpResponseMessage Get(string accountId);

        void Reset(string accountId);

        void CheckAccountIsLocked(string accountId);
    }
}