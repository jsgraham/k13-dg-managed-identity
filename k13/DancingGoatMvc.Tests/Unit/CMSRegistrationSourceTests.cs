using System;

using CMS.Core;
using CMS.Tests;

using Autofac;

using NUnit.Framework;

namespace DancingGoat.Tests.Unit
{
    [TestFixture]
    public class CMSRegistrationSourceTests : ContainerNotBuiltUnitTests
    {
        [Test]
        public void CMSRegistrationSourceTests_ServiceForInjectionIsRegisteredInCMS_ServiceIsResolvedCorrectly()
        {
            // Register service to CMS
            Service.Use(typeof(ITestService), typeof(TestService));

            EnsureServiceContainer();

            // Register service which requires ITestService in constructor and also register fallback CMS resolver 
            var builder = new ContainerBuilder();
            builder.RegisterSource(new CMSRegistrationSource());
            builder.RegisterGeneric(typeof(TestGenericService<>)).As(typeof(ITestGenericService<>));

            var container = builder.Build();

            var autofacResolve = container.Resolve<ITestGenericService<ITestService>>();

            Assert.IsNotNull(autofacResolve.TestService);
        }


        [Test]
        public void CMSRegistrationSourceTests_BothServicesInCMS_ServiceIsSingleton_ResolvedServicesReturnsSameValue()
        {
            // Register both services in CMS
            Service.Use(typeof(ITestService), typeof(TestService));
            Service.Use(typeof(ITestGenericService<>), typeof(TestGenericService<>));

            EnsureServiceContainer();
            
            // Register fallback CMS resolver
            var builder = new ContainerBuilder();
            builder.RegisterSource(new CMSRegistrationSource());

            var container = builder.Build();

            var firstResolve = container.Resolve<ITestGenericService<ITestService>>();
            var secondResolve = container.Resolve<ITestGenericService<ITestService>>();

            var firstGuid = firstResolve.TestService.Guid;
            var secondGuid = secondResolve.TestService.Guid;

            Assert.AreEqual(firstGuid, secondGuid, "ITestService implementation is not singleton.");
        }


        [Test]
        public void CMSRegistrationSourceTests_BothServicesInCMS_ServiceIsTransient_ResolvedServicesReturnsDifferentValue()
        {
            // Register both services in CMS
            Service.Use(typeof(ITestService), typeof(TestService), transient: true);
            Service.Use(typeof(ITestGenericService<>), typeof(TestGenericService<>));

            EnsureServiceContainer();

            // Register fallback CMS resolver
            var builder = new ContainerBuilder();
            builder.RegisterSource(new CMSRegistrationSource());

            var container = builder.Build();

            var firstResolve = container.Resolve<ITestGenericService<ITestService>>();
            var secondResolve = container.Resolve<ITestGenericService<ITestService>>();

            var firstGuid = firstResolve.TestService.Guid;
            var secondGuid = secondResolve.TestService.Guid;

            Assert.AreEqual(firstGuid, secondGuid, "ITestService implementation is not singleton.");
        }


        [Test]
        public void ServiceIsTransient_ResolvedServicesReturnsDifferentValue()
        {
            // Register both services in CMS
            Service.Use<ITestService>(() => new TestService(), nameof(TestService), true);

            EnsureServiceContainer();

            // Register fallback CMS resolver
            var builder = new ContainerBuilder();
            builder.RegisterSource(new CMSRegistrationSource());

            var container = builder.Build();

            var service1 = container.Resolve<ITestService>();
            var service2 = container.Resolve<ITestService>();

            Assert.AreNotEqual(service1, service2, "ITestService implementation is not singleton.");
        }
    }


    public interface ITestService
    {
        Guid Guid { get; }
    }


    public interface ITestGenericService<T> where T : ITestService
    {
        ITestService TestService { get; }
    }


    /// <summary>
    /// Implementation for testing singleton vs transient dependency resolution.
    /// </summary>
    public class TestService : ITestService
    {
        private readonly Guid guid = Guid.NewGuid();


        public Guid Guid => guid;
    }


    /// <summary>
    /// Implementation to test constructor injection.
    /// </summary>
    public class TestGenericService<T> : ITestGenericService<T> where T : ITestService
    {
        public ITestService TestService { get; }


        public TestGenericService(ITestService testSevice)
        {
            TestService = testSevice;
        }
    }
}
