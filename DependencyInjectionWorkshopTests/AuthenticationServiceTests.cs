namespace DependencyInjectionWorkshopTests
{
    using DependencyInjectionWorkshop.Decorator;
    using DependencyInjectionWorkshop.Exceptions;
    using DependencyInjectionWorkshop.Models;
    using DependencyInjectionWorkshop.Repo;
    using DependencyInjectionWorkshop.Service;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class AuthenticationServiceTests
    {
        private IProfile _profile;

        private IOtp _otp;

        private IHash _hash;

        private INotification _notification;

        private IFailedCounter _failedCounter;

        private ILogger _logger;

        private IAuthentication _authentication;

        private const string DefaultAccountId = "joey";

        private const string DefaultHashedPassword = "my hashed password";

        private const string DefaultOtp = "123456";

        private const string DefaultPassword = "pw";

        private const int DefaultFailedCount = 91;

        [SetUp]
        public void Setup()
        {
            _profile = Substitute.For<IProfile>();
            _otp = Substitute.For<IOtp>();
            _hash = Substitute.For<IHash>();
            _notification = Substitute.For<INotification>();
            _failedCounter = Substitute.For<IFailedCounter>();
            _logger = Substitute.For<ILogger>();

            var authenticationService = new AuthenticationService(_profile, _hash, _otp);

            var notificationDecorator = new NotificationDecorator(authenticationService, _notification);
            var failedCounterDecorator = new FailedCounterDecorator(notificationDecorator, _failedCounter);
            var logDecorator = new LogDecorator(failedCounterDecorator, _logger, _failedCounter);

            _authentication = logDecorator;
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

        [Test]
        public void is_invalid_when_wrong_otp()
        {
            GivenPassword(DefaultAccountId, DefaultHashedPassword);
            GivenHash(DefaultPassword, DefaultHashedPassword);
            GivenOtp(DefaultAccountId, DefaultOtp);

            var isValid = WhenVerify(DefaultAccountId, DefaultPassword, "wrong otp");

            ShouldBeInvalid(isValid);
        }

        [Test]
        public void notify_user_when_invalid()
        {
            WhenInvalid();
            ShouldNotifyUser();
        }

        private void ShouldNotifyUser()
        {
            _notification.Received(1).PushMessage(Arg.Any<string>());
        }

        private bool WhenValid()
        {
            GivenPassword(DefaultAccountId, DefaultHashedPassword);
            GivenHash(DefaultPassword, DefaultHashedPassword);
            GivenOtp(DefaultAccountId, DefaultOtp);

            var isValid = WhenVerify(DefaultAccountId, DefaultPassword, DefaultOtp);
            return isValid;
        }

        private bool WhenInvalid()
        {
            GivenPassword(DefaultAccountId, DefaultHashedPassword);
            GivenHash(DefaultPassword, DefaultHashedPassword);
            GivenOtp(DefaultAccountId, DefaultOtp);

            return WhenVerify(DefaultAccountId, DefaultPassword, "wrong otp");
        }

        [Test]
        public void log_account_failed_count_when_invalid()
        {
            GivenFailedCount(DefaultFailedCount);

            WhenInvalid();

            LogShouldContains(DefaultAccountId, DefaultFailedCount);
        }

        [Test]
        public void reset_failed_count_when_valid()
        {
            WhenValid();
            ShouldResetFailedCounter();
        }

        private void ShouldResetFailedCounter()
        {
            _failedCounter.Received(1).Reset(Arg.Any<string>());
        }

        [Test]
        public void add_failed_count_when_invalid()
        {
            WhenInvalid();
            ShouldAddFailedCount();
        }

        [Test]
        public void account_is_locked()
        {
            _failedCounter.CheckAccountIsLocked(DefaultAccountId).ReturnsForAnyArgs(true);

            TestDelegate action = () => _authentication.Verify(DefaultAccountId, DefaultPassword, DefaultOtp);
            Assert.Throws<FailedTooManyTimesException>(action);
        }

        private void ShouldAddFailedCount()
        {
            _failedCounter.ReceivedWithAnyArgs(1).Add(DefaultAccountId);
        }

        private void LogShouldContains(string accountId, int failedCount)
        {
            _logger.Received(1).Info(Arg.Is<string>(m => m.Contains(accountId) && m.Contains(failedCount.ToString())));
        }

        private void GivenFailedCount(int failedCount)
        {
            _failedCounter.Get(DefaultAccountId).ReturnsForAnyArgs(failedCount);
        }

        private static void ShouldBeInvalid(bool isValid)
        {
            Assert.IsFalse(isValid);
        }

        private static void ShouldBeValid(bool isValid)
        {
            Assert.IsTrue(isValid);
        }

        private bool WhenVerify(string accountId, string password, string otp)
        {
            return _authentication.Verify(accountId, password, otp);
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