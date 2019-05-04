namespace DependencyInjectionWorkshop.Decorator
{
    using DependencyInjectionWorkshop.Models;

    public class NotificationDecorator : IAuthentication
    {
        private readonly IAuthentication _authentication;

        private readonly INotification _notification;

        public NotificationDecorator(IAuthentication authentication, INotification notification)
        {
            _authentication = authentication;
            _notification = notification;
        }

        private void NotificationVerify(string accountId)
        {
            _notification.PushMessage($"account:{accountId}, AuthFailed!");
        }

        public bool Verify(string accountId, string password, string otp)
        {
            var isValid = _authentication.Verify(accountId, password, otp);
            if (!isValid)
            {
                NotificationVerify(accountId);
            }

            return isValid;
        }
    }
}