namespace DependencyInjectionWorkshop.Models
{
    using DependencyInjectionWorkshop.Exceptions;
    using DependencyInjectionWorkshop.Repo;
    using DependencyInjectionWorkshop.Service;

    public class AuthenticationService : IAuthentication
    {
        private readonly IProfile _profile;
        private readonly IHash _hash;
        private readonly IOtp _otp;

        public AuthenticationService(IProfile profile, IHash hash, IOtp otp)
        {
            _profile = profile;
            _hash = hash;
            _otp = otp;
        }

        public bool Verify(string accountId, string password, string otp)
        {
            var passwordFromDB = _profile.GetPassword(accountId);

            var hashPassword = _hash.GetHash(password);

            var currentOTP = _otp.GetCurrentOTP(accountId);

            return passwordFromDB == hashPassword && currentOTP == otp;
        }
    }
}