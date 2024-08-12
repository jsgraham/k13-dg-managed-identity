using System.Collections.Generic;

using CMS.Base;
using CMS.ContactManagement;
using CMS.ContactManagement.Internal;
using CMS.Core;
using CMS.Membership;
using CMS.Tests;

using DancingGoat.Personalization;

using NSubstitute;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace DancingGoat.Tests
{
    [TestFixture]
    public class IsInContactGroupConditionTypeTests : ContainerNotBuiltUnitTests
    {
        private const int CONTACT_ID = 1; 
        private const int CONTACT_GROUP_ID = 1;
        private const int CONTACT_IN_CONTACT_GROUP_ID = 2;
        private const string CONTACT_GROUP_NAME = "TestGroup";

        private ContactInfo contactInContactGroup;
        private ContactInfo contact;

        private IContactProcessingChecker checker;
        private ICurrentContactProvider currentContactProvider;

        [SetUp]
        public void SetUp()
        {
            var provider = Fake<ContactInfo, ContactInfoProvider>();
            contact = new ContactInfo
            {
                ContactID = CONTACT_ID,
            };
            contactInContactGroup = new ContactInfo
            {
                ContactID = CONTACT_IN_CONTACT_GROUP_ID,
            };
            provider.WithData(contactInContactGroup, contact);

            Fake<ContactGroupInfo, ContactGroupInfoProvider>().WithData(
                new ContactGroupInfo
                {
                    ContactGroupID = CONTACT_GROUP_ID,
                    ContactGroupName = CONTACT_GROUP_NAME,
                    ContactGroupDisplayName = "Test Group"
                }
            );
            Fake<ContactGroupMemberInfo, ContactGroupMemberInfoProvider>().WithData(
                new ContactGroupMemberInfo
                {
                    ContactGroupMemberID = 1,
                    ContactGroupMemberContactGroupID = CONTACT_GROUP_ID,
                    ContactGroupMemberRelatedID = CONTACT_IN_CONTACT_GROUP_ID,
                    ContactGroupMemberType = ContactGroupMemberTypeEnum.Contact
                }
            );

            var user = Substitute.For<CurrentUserInfo>();
            MembershipContext.AuthenticatedUser = user;

            currentContactProvider = Substitute.For<ICurrentContactProvider>();
            currentContactProvider.GetExistingContact(Arg.Any<IUserInfo>(), Arg.Any<bool>()).Returns(contactInContactGroup);
            Service.Use<ICurrentContactProvider>(currentContactProvider);

            checker = Substitute.For<IContactProcessingChecker>();
            checker.CanProcessContactInCurrentContext().Returns(true);
            Service.Use<IContactProcessingChecker>(checker);

            Service.Use<IContactInfoProvider>(ContactInfo.Provider);

            EnsureServiceContainer();
        }


        private static IEnumerable<TestCaseData> noContactGroupSelectedSource
        {
            get
            {
                yield return new TestCaseData(CONTACT_ID, Is.True).SetName("Evaluate_NoContactGroupSelectedAndCurrentContactIsNotInContactGroup_ReturnsTrue");
                yield return new TestCaseData(CONTACT_IN_CONTACT_GROUP_ID, Is.False).SetName("Evaluate_NoContactGroupSelectedAndCurrentContactIsInContactGroup_ReturnsFalse");
            }
        }


        [TestCaseSource(nameof(noContactGroupSelectedSource))]
        public void Evaluate_NoContactGroupSelected_ReturnsCorrectResult(int contactId, Constraint result)
        {
            var currentContact = ContactInfo.Provider.Get(contactId);
            currentContactProvider.GetCurrentContact(Arg.Any<IUserInfo>(), Arg.Any<bool>())
                                  .Returns(currentContact);

            var evaluatorEmpty = new IsInContactGroupConditionType
            {
                SelectedContactGroups = new List<string>()
            };

            var evaluatorNull = new IsInContactGroupConditionType
            {
                SelectedContactGroups = null
            };

            Assert.Multiple(() =>
            {
                Assert.That(() => evaluatorEmpty.Evaluate(), result);
                Assert.That(() => evaluatorNull.Evaluate(), result);
            });
        }


        [Test]
        public void Evaluate_IsInContactGroup_ReturnsTrue()
        {
            currentContactProvider.GetCurrentContact(Arg.Any<IUserInfo>(), Arg.Any<bool>())
                                  .Returns(contactInContactGroup);

            var evaluator = new IsInContactGroupConditionType
            {
                SelectedContactGroups = new List<string> { CONTACT_GROUP_NAME }
            };

            Assert.That(evaluator.Evaluate(), Is.True);
        }


        [Test]
        public void Evaluate_IsNotInAnyContactGroup_ReturnsFalse()
        {
            currentContactProvider.GetCurrentContact(Arg.Any<IUserInfo>(), Arg.Any<bool>()).Returns(contact);
            var evaluator = new IsInContactGroupConditionType
            {
                SelectedContactGroups = new List<string> { CONTACT_GROUP_NAME }
            };

            Assert.That(evaluator.Evaluate(), Is.False);
        }


        [Test]
        public void Evaluate_CurrentContactIsNull_ReturnsFalse()
        {
            checker.CanProcessContactInCurrentContext().Returns(false);

            var evaluator = new IsInContactGroupConditionType();

            Assert.Multiple(() =>
            {
                Assert.That(ContactManagementContext.GetCurrentContact(), Is.Null);
                Assert.That(evaluator.Evaluate(), Is.False);
            });
        }
    }
}
