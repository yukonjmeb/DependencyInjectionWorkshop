﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyConsole
{
    using Autofac;

    using DependencyInjectionWorkshop.Decorator;
    using DependencyInjectionWorkshop.Models;
    using DependencyInjectionWorkshop.Repo;
    using DependencyInjectionWorkshop.Service;

    public class Program
    {
        private static bool isValid;

        private static IContainer _container;

        public static void Main(string[] args)
        {
            RegisterContainer();
            IAuthentication authentication = _container.Resolve<IAuthentication>();
            isValid = authentication.Verify("joey", "pw", "123456");

            Console.WriteLine(isValid);
            Console.ReadKey();
        }

        private static void RegisterContainer()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<FakeOtp>().As<IOtp>();
            containerBuilder.RegisterType<FakeHash>().As<IHash>();
            containerBuilder.RegisterType<FakeProfile>().As<IProfile>();
            containerBuilder.RegisterType<FakeSlack>().As<INotification>();
            containerBuilder.RegisterType<ConsoleAdapter>().As<ILogger>();
            containerBuilder.RegisterType<FakeFailedCounter>().As<IFailedCounter>();

            containerBuilder.RegisterType<LogDecorator>();
            containerBuilder.RegisterType<NotificationDecorator>();
            containerBuilder.RegisterType<FailedCounterDecorator>();

            containerBuilder.RegisterType<AuthenticationService>().As<IAuthentication>();

            containerBuilder.RegisterDecorator<NotificationDecorator, IAuthentication>();
            containerBuilder.RegisterDecorator<FailedCounterDecorator, IAuthentication>();
            containerBuilder.RegisterDecorator<LogDecorator, IAuthentication>();

            _container = containerBuilder.Build();
        }

        internal class FakeSlack : INotification
        {
            public void PushMessage(string message)
            {
                Console.WriteLine($"{nameof(FakeSlack)}.{nameof(PushMessage)}({message})");
            }
        }

        internal class FakeFailedCounter : IFailedCounter
        {
            public void Reset(string accountId)
            {
                Console.WriteLine($"{nameof(FakeFailedCounter)}.{nameof(Reset)}({accountId})");
            }

            public void Add(string accountId)
            {
                Console.WriteLine($"{nameof(FakeFailedCounter)}.{nameof(Add)}({accountId})");
            }

            public int Get(string accountId)
            {
                Console.WriteLine($"{nameof(FakeFailedCounter)}.{nameof(Get)}({accountId})");
                return 91;
            }

            public bool CheckAccountIsLocked(string accountId)
            {
                Console.WriteLine($"{nameof(FakeFailedCounter)}.{nameof(CheckAccountIsLocked)}({accountId})");
                return false;
            }
        }

        internal class FakeOtp : IOtp
        {
            public string GetCurrentOtp(string accountId)
            {
                Console.WriteLine($"{nameof(FakeOtp)}.{nameof(GetCurrentOtp)}({accountId})");
                return "123456";
            }
        }

        internal class FakeHash : IHash
        {
            public string GetHash(string plainText)
            {
                Console.WriteLine($"{nameof(FakeHash)}.{nameof(GetHash)}({plainText})");
                return "my hashed password";
            }
        }

        internal class FakeProfile : IProfile
        {
            public string GetPassword(string accountId)
            {
                Console.WriteLine($"{nameof(FakeProfile)}.{nameof(GetPassword)}({accountId})");
                return "my hashed password";
            }
        }
    }
}
