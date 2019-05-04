namespace DependencyInjectionWorkshop.Models
{
    using DependencyInjectionWorkshop.Repo;
    using DependencyInjectionWorkshop.Service;

    public class AuthenticationService
    {
        private readonly IProfile _profile;
        private readonly IFailedCounter _failedCounter;
        private readonly IHash _hash;
        private readonly IOTP _otp;
        private readonly ILogger _logger;
        private readonly INotification _notification;

        public AuthenticationService(IProfile profile, IFailedCounter failedCounter, IHash hash, IOTP otp, ILogger logger, INotification notification)
        {
            _profile = profile;
            _failedCounter = failedCounter;
            _hash = hash;
            _otp = otp;
            _logger = logger;
            _notification = notification;
        }

        public AuthenticationService()
        {
            _profile = new Profile();
            _failedCounter = new FailedCounter();
            _hash = new Hash();
            _otp = new OTP();
            _logger = new Logger();
            _notification = new Notification();
        }

        public bool Verify(string accountId, string password, string otp)
        {
            _failedCounter.CheckAccountIsLocked(accountId);

            var passwordFromDB = _profile.GetPassword(accountId);

            var hashPassword = _hash.GetHash(password);

            var currentOTP = _otp.GetCurrentOTP(accountId);

            if (passwordFromDB == hashPassword && currentOTP == otp)
            {
                _failedCounter.Reset(accountId);
                return true;
            }
            else
            {
                _failedCounter.Add(accountId);

                var failedCountResponse = _failedCounter.Get(accountId);

                _logger.Info(accountId, failedCountResponse);

                _notification.SlackAdapter();

                return false;
            }
        }
    }
}