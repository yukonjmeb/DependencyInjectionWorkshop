namespace DependencyInjectionWorkshop.Models
{
    using DependencyInjectionWorkshop.Exceptions;
    using DependencyInjectionWorkshop.Repo;
    using DependencyInjectionWorkshop.Service;

    public class AuthenticationService : IAuthentication
    {
        private readonly IProfile _profile;
        private readonly IFailedCounter _failedCounter;
        private readonly IHash _hash;
        private readonly IOTP _otp;
        private readonly ILogger _logger;

        public AuthenticationService(IProfile profile, IFailedCounter failedCounter, IHash hash, IOTP otp, ILogger logger)
        {
            _profile = profile;
            _failedCounter = failedCounter;
            _hash = hash;
            _otp = otp;
            _logger = logger;
        }

        public bool Verify(string accountId, string password, string otp)
        {
            if (_failedCounter.CheckAccountIsLocked(accountId))
            {
                throw new FailedTooManyTimesException();
            }

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

                _logger.Info($"account:{accountId}, failedCount:{_failedCounter.Get(accountId)}");

                return false;
            }
        }
    }
}