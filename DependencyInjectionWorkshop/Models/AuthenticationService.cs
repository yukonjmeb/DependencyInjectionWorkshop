namespace DependencyInjectionWorkshop.Models
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Net.Http;

    using Dapper;

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

            var CurrentOTP = _otpService.GetCurrentOtp(accountId);

            if (passwordFromDB == hashPassword && CurrentOTP == otp)
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

    public class FailedTooManyTimesException : Exception
    {
        public FailedTooManyTimesException()
        {
        }
    }
}