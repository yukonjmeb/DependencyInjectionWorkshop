namespace DependencyInjectionWorkshop.Models
{
    using DependencyInjectionWorkshop.Repo;
    using DependencyInjectionWorkshop.Service;

    public class AuthenticationService
    {
        private readonly ProfileRepo _profileRepo = new ProfileRepo();
        private readonly SHA256Adapter _sha256Adapter = new SHA256Adapter();
        private readonly OtpService _otpService = new OtpService();
        private readonly FailedCounter _failedCounter = new FailedCounter();
        private readonly NLogAdapter _nLogAdapter = new NLogAdapter();
        private readonly SlackAdapter _slackAdapter = new SlackAdapter();

        public bool Verify(string accountId, string password, string otp)
        {
            _failedCounter.CheckAccountIsLocked(accountId);

            var passwordFromDB = _profileRepo.GetPasswordFromDB(accountId);

            var hashPassword = _sha256Adapter.GetHashPassword(password);

            var currentOTP = _otpService.GetCurrentOTP(accountId);

            if (passwordFromDB == hashPassword && currentOTP == otp)
            {
                _failedCounter.ResetFailedCounter(accountId);
                return true;
            }
            else
            {
                _failedCounter.AddFailedCount(accountId);

                var failedCountResponse = _failedCounter.FailedCount(accountId);

                _nLogAdapter.LogFailedCount(accountId, failedCountResponse);

                _slackAdapter.NotifyAuthFailed();

                return false;
            }
        }
    }
}