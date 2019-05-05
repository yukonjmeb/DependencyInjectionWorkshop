namespace DependencyInjectionWorkshop.Decorator
{
    using DependencyInjectionWorkshop.Models;

    public class NotificationBaseDecorator : AuthenticationBaseDecorator 
    {
        private readonly INotification _notification;

        public NotificationBaseDecorator(IAuthentication authentication, INotification notification)
            : base(authentication)
        {
            _notification = notification;
        }

        private void NotificationVerify(string accountId)
        {
            _notification.PushMessage($"account:{accountId}, AuthFailed!");
        }

        public override bool Verify(string accountId, string password, string otp)
        {
            var isValid = base.Verify(accountId, password, otp);
            if (!isValid)
            {
                NotificationVerify(accountId);
            }

            return isValid;
        }
    }
}