using CMS.Base;
using CMS.ContactManagement;
using CMS.ContactManagement.Internal;
using CMS.Core;
using CMS.Membership;
using CMS.Tests;
using CMS.Personas;

using DancingGoat.Personalization;

using NSubstitute;

using NUnit.Framework;

namespace DancingGoat.Tests
{
    [TestFixture]
    public class IsInPersonaEvaluatorTests : ContainerNotBuiltUnitTests
    {
        private const int CONTACT_ID = 1;
        private const int CONTACT_WITH_PERSONA_ID = 2;
        private const int PERSONA_ID = 1;
        private const string PERSONA_NAME = "TestPersona";
        private const int OTHER_PERSONA_ID = 2;
        private const string OTHER_PERSONA_NAME = "OtherPersona";
        private const int SCORE_ID = 1;

        private ContactInfo contact;

        private IContactProcessingChecker checker;
        private ICurrentContactProvider service;


        [SetUp]
        public void SetUp()
        {
            var provider = Fake<ContactInfo, ContactInfoProvider>();
            contact = new ContactInfo
            {
                ContactID = CONTACT_ID,
            };
            var contactInPersona = new ContactInfo
            {
                ContactID = CONTACT_WITH_PERSONA_ID,
                ContactPersonaID = PERSONA_ID
            };
            provider.WithData(contactInPersona, contact);

            Fake<ScoreInfo, ScoreInfoProvider>().WithData(
                new ScoreInfo
                {
                    ScoreID = SCORE_ID,
                    ScoreName = "TestScore",
                    ScoreDisplayName = "Test Score",
                    ScoreEnabled = true,
                    ScorePersonaID = PERSONA_ID
                });

            Fake<PersonaInfo, PersonaInfoProvider>().WithData(
                new PersonaInfo
                {
                    PersonaID = PERSONA_ID,
                    PersonaDisplayName = "Test Persona",
                    PersonaName = PERSONA_NAME,
                    PersonaEnabled = true,
                    PersonaPointsThreshold = 1
                }, 
                new PersonaInfo
                {
                    PersonaID = OTHER_PERSONA_ID,
                    PersonaDisplayName = "Other Persona",
                    PersonaName = OTHER_PERSONA_NAME,
                    PersonaEnabled = true,
                    PersonaPointsThreshold = 1
                });

            var user = Substitute.For<CurrentUserInfo>();
            MembershipContext.AuthenticatedUser = user;

            service = Substitute.For<ICurrentContactProvider>();
            service.GetExistingContact(Arg.Any<IUserInfo>(), Arg.Any<bool>()).Returns(contactInPersona);
            Service.Use<ICurrentContactProvider>(service);

            checker = Substitute.For<IContactProcessingChecker>();
            checker.CanProcessContactInCurrentContext().Returns(true);
            Service.Use<IContactProcessingChecker>(checker);

            EnsureServiceContainer();
        }


        [Test]
        public void Evaluate_IsInSelectedPersona_ReturnsTrue()
        {
            var condition = new IsInPersonaConditionType { PersonaName = PERSONA_NAME };

            Assert.That(condition.Evaluate(), Is.True);
        }


        [Test]
        public void Evaluate_IsNotInSelectedPersona_ReturnsFalse()
        {
            var condition = new IsInPersonaConditionType { PersonaName = OTHER_PERSONA_NAME };

            Assert.That(condition.Evaluate(), Is.False);
        }


        [Test]
        public void Evaluate_IsNotInPersona_ReturnsFalse()
        {
            service.GetExistingContact(Arg.Any<IUserInfo>(), Arg.Any<bool>()).Returns(contact);

            var condition = new IsInPersonaConditionType { PersonaName = PERSONA_NAME };

            Assert.That(condition.Evaluate(), Is.False);
        }


        [Test]
        public void Evaluate_CurrentContactIsNull_ReturnsFalse()
        {
            checker.CanProcessContactInCurrentContext().Returns(false);

            var condition = new IsInPersonaConditionType { PersonaName = PERSONA_NAME };

            Assert.Multiple(() =>
            {
                Assert.That(ContactManagementContext.GetCurrentContact(), Is.Null);
                Assert.That(condition.Evaluate(), Is.False);
            });
        }
    }
}
