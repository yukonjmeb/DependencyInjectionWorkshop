using DependencyInjectionWorkshop.Models;

using NUnit.Framework;

namespace DependencyInjectionWorkshopTests
{
    using DependencyInjectionWorkshop.Repo;
    using DependencyInjectionWorkshop.Service;

    using NSubstitute;

    [TestFixture]
    public class AuthenticationServiceTests
    {
        private IProfile _profile;

        private IOTP _otp;

        private IHash _hash;

        private INotification _notification;

        private IFailedCounter _failedCounter;

        private ILogger _logger;

        private AuthenticationService _authenticationService;

        private const string DefaultAccountId = "joey";

        private const string DefaultHashedPassword = "my hashed password";

        private const string DefaultOtp = "123456";

        private const string DefaultPassword = "pw";

        [SetUp]
        public void Setup()
        {
            _profile = Substitute.For<IProfile>();
            _otp = Substitute.For<IOTP>();
            _hash = Substitute.For<IHash>();
            _notification = Substitute.For<INotification>();
            _failedCounter = Substitute.For<IFailedCounter>();
            _logger = Substitute.For<ILogger>();

            _authenticationService = new AuthenticationService(
                _profile,
                _failedCounter,
                _hash,
                _otp,
                _logger,
                _notification);
        }

        [Test]
        public void is_valid()
        {
            GivenOtp(DefaultAccountId, DefaultOtp);
            GivenPassword(DefaultAccountId, DefaultHashedPassword);
            GivenHash(DefaultPassword, DefaultHashedPassword);

            var isValid = WhenVerify(DefaultAccountId, DefaultPassword, DefaultOtp);

            ShouldBeValid(isValid);
        }

        private static void ShouldBeValid(bool isValid)
        {
            Assert.IsTrue(isValid);
        }

        private bool WhenVerify(string accountId, string password, string otp)
        {
            return _authenticationService.Verify(accountId, password, otp);
        }

        private void GivenHash(string password, string hash)
        {
            _hash.GetHash(password).ReturnsForAnyArgs(hash);
        }

        private void GivenPassword(string accountId, string hash)
        {
            _profile.GetPassword(accountId).ReturnsForAnyArgs(hash);
        }

        private void GivenOtp(string accountId, string otp)
        {
            _otp.GetCurrentOTP(accountId).ReturnsForAnyArgs(otp);
        }
    }
}