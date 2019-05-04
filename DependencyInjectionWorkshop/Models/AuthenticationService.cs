namespace DependencyInjectionWorkshop.Models
{
    using DependencyInjectionWorkshop.Repo;
    using DependencyInjectionWorkshop.Service;

    public class AuthenticationService
    {
        private readonly IProfile _profile;
        private readonly FailedCounter _failedCounter;
        private readonly SHA256Adapter _sha256Adapter;
        private readonly OtpService _otpService;
        private readonly NLogAdapter _nLogAdapter;
        private readonly SlackAdapter _slackAdapter;

        public AuthenticationService(IProfile profile, FailedCounter failedCounter, SHA256Adapter sha256Adapter, OtpService otpService, NLogAdapter nLogAdapter, SlackAdapter slackAdapter)
        {
            _profile = profile;
            _failedCounter = failedCounter;
            _sha256Adapter = sha256Adapter;
            _otpService = otpService;
            _nLogAdapter = nLogAdapter;
            _slackAdapter = slackAdapter;
        }

        public AuthenticationService()
        {
            _profile = new Profile();
            _failedCounter = new FailedCounter();
            _sha256Adapter = new SHA256Adapter();
            _otpService = new OtpService();
            _nLogAdapter = new NLogAdapter();
            _slackAdapter = new SlackAdapter();
        }

        public bool Verify(string accountId, string password, string otp)
        {
            var failedCounter = _failedCounter;
            failedCounter.CheckAccountIsLocked(accountId);

            var passwordFromDB = _profile.GetPassword(accountId);

            var hashPassword = _sha256Adapter.GetHashPassword(password);

            var currentOTP = _otpService.GetCurrentOTP(accountId);

            if (passwordFromDB == hashPassword && currentOTP == otp)
            {
                failedCounter.ResetFailedCounter(accountId);
                return true;
            }
            else
            {
                failedCounter.AddFailedCount(accountId);

                var failedCountResponse = failedCounter.FailedCount(accountId);

                _nLogAdapter.LogFailedCount(accountId, failedCountResponse);

                _slackAdapter.NotifyAuthFailed();

                return false;
            }
        }
    }
}