using System;
using Xunit;
using Moq;
using BackOffice.Domain.Users;
using BackOffice.Domain.Patients;
using BackOffice.Domain.Shared;

namespace BackOffice.DomainTests.Users.Tests
{
    public class UserTest
    {
        [Fact]
        public void User_Should_Throw_Exception_When_Email_Is_Empty()
        {
            var mockPhoneNumber = new Mock<PhoneNumber>(123456789);
            var mockName = new Mock<Name>("John");
            var mockLastName = new Mock<Name>("Doe");
            var mockFullName = new Mock<Name>("John Doe");

            Assert.Throws<BusinessRuleValidationException>(() => new User("", "Admin", mockPhoneNumber.Object, mockName.Object, mockLastName.Object, mockFullName.Object));
        }

        [Fact]
        public void User_Should_Throw_Exception_When_Role_Is_Empty()
        {
            var mockPhoneNumber = new Mock<PhoneNumber>(123456789);
            var mockName = new Mock<Name>("John");
            var mockLastName = new Mock<Name>("Doe");
            var mockFullName = new Mock<Name>("John Doe");

            Assert.Throws<BusinessRuleValidationException>(() => new User("john.doe@example.com", "", mockPhoneNumber.Object, mockName.Object, mockLastName.Object, mockFullName.Object));
        }

        [Fact]
        public void User_Should_Throw_Exception_When_PhoneNumber_Is_Null()
        {
            var mockName = new Mock<Name>("John");
            var mockLastName = new Mock<Name>("Doe");
            var mockFullName = new Mock<Name>("John Doe");

            Assert.Throws<BusinessRuleValidationException>(() => new User("john.doe@example.com", "Admin", null, mockName.Object, mockLastName.Object, mockFullName.Object));
        }

        [Fact]
        public void User_Should_Throw_Exception_When_FirstName_Is_Null()
        {
            var mockPhoneNumber = new Mock<PhoneNumber>(123456789);
            var mockLastName = new Mock<Name>("Doe");
            var mockFullName = new Mock<Name>("John Doe");

            Assert.Throws<BusinessRuleValidationException>(() => new User("john.doe@example.com", "Admin", mockPhoneNumber.Object, null, mockLastName.Object, mockFullName.Object));
        }

        [Fact]
        public void User_Should_Throw_Exception_When_LastName_Is_Null()
        {
            var mockPhoneNumber = new Mock<PhoneNumber>(123456789);
            var mockName = new Mock<Name>("John");
            var mockFullName = new Mock<Name>("John Doe");

            Assert.Throws<BusinessRuleValidationException>(() => new User("john.doe@example.com", "Admin", mockPhoneNumber.Object, mockName.Object, null, mockFullName.Object));
        }

        [Fact]
        public void User_Should_Throw_Exception_When_FullName_Is_Null()
        {
            var mockPhoneNumber = new Mock<PhoneNumber>(123456789);
            var mockName = new Mock<Name>("John");
            var mockLastName = new Mock<Name>("Doe");

            Assert.Throws<BusinessRuleValidationException>(() => new User("john.doe@example.com", "Admin", mockPhoneNumber.Object, mockName.Object, mockLastName.Object, null));
        }

        [Fact]
        public void User_Should_Generate_Activation_Token()
        {
            var mockPhoneNumber = new Mock<PhoneNumber>(123456789);
            var mockName = new Mock<Name>("John");
            var mockLastName = new Mock<Name>("Doe");
            var mockFullName = new Mock<Name>("John Doe");

            var user = new User("john.doe@example.com", "Admin", mockPhoneNumber.Object, mockName.Object, mockLastName.Object, mockFullName.Object);
            user.GenerateActivationToken();

            Assert.NotNull(user.ActivationToken);
            Assert.True(user.TokenExpiration.HasValue);
        }

        [Fact]
        public void User_Should_Validate_Activation_Token()
        {
            var mockPhoneNumber = new Mock<PhoneNumber>(123456789);
            var mockName = new Mock<Name>("John");
            var mockLastName = new Mock<Name>("Doe");
            var mockFullName = new Mock<Name>("John Doe");

            var user = new User("john.doe@example.com", "Admin", mockPhoneNumber.Object, mockName.Object, mockLastName.Object, mockFullName.Object);
            user.GenerateActivationToken();

            Assert.True(user.IsActivationTokenValid());
        }

        [Fact]
        public void User_Should_Change_Role()
        {
            var mockPhoneNumber = new Mock<PhoneNumber>(123456789);
            var mockName = new Mock<Name>("John");
            var mockLastName = new Mock<Name>("Doe");
            var mockFullName = new Mock<Name>("John Doe");

            var user = new User("john.doe@example.com", "Admin", mockPhoneNumber.Object, mockName.Object, mockLastName.Object, mockFullName.Object);
            user.MarkAsActive();
            user.ChangeRole("User");

            Assert.Equal("User", user.Role);
        }

        [Fact]
        public void User_Should_Throw_Exception_When_Changing_Role_Of_Inactive_User()
        {
            var mockPhoneNumber = new Mock<PhoneNumber>(123456789);
            var mockName = new Mock<Name>("John");
            var mockLastName = new Mock<Name>("Doe");
            var mockFullName = new Mock<Name>("John Doe");

            var user = new User("john.doe@example.com", "Admin", mockPhoneNumber.Object, mockName.Object, mockLastName.Object, mockFullName.Object);

            Assert.Throws<BusinessRuleValidationException>(() => user.ChangeRole("User"));
        }

        [Fact]
        public void User_Should_Mark_As_Inactive()
        {
            var mockPhoneNumber = new Mock<PhoneNumber>(123456789);
            var mockName = new Mock<Name>("John");
            var mockLastName = new Mock<Name>("Doe");
            var mockFullName = new Mock<Name>("John Doe");

            var user = new User("john.doe@example.com", "Admin", mockPhoneNumber.Object, mockName.Object, mockLastName.Object, mockFullName.Object);
            user.MarkAsInactive();

            Assert.False(user.Active);
        }

        [Fact]
        public void User_Should_Mark_As_Active()
        {
            var mockPhoneNumber = new Mock<PhoneNumber>(123456789);
            var mockName = new Mock<Name>("John");
            var mockLastName = new Mock<Name>("Doe");
            var mockFullName = new Mock<Name>("John Doe");

            var user = new User("john.doe@example.com", "Admin", mockPhoneNumber.Object, mockName.Object, mockLastName.Object, mockFullName.Object);
            user.MarkAsActive();

            Assert.True(user.Active);
        }

        [Fact]
        public void User_Should_Mark_As_To_Be_Deleted()
        {
            var mockPhoneNumber = new Mock<PhoneNumber>(123456789);
            var mockName = new Mock<Name>("John");
            var mockLastName = new Mock<Name>("Doe");
            var mockFullName = new Mock<Name>("John Doe");

            var user = new User("john.doe@example.com", "Admin", mockPhoneNumber.Object, mockName.Object, mockLastName.Object, mockFullName.Object);
            user.MarkDeleteAsActive();

            Assert.True(user.IsToBeDeleted);
        }

        [Fact]
        public void User_Should_Update_Name()
        {
            var mockPhoneNumber = new Mock<PhoneNumber>(123456789);
            var mockName = new Mock<Name>("John");
            var mockLastName = new Mock<Name>("Doe");
            var mockFullName = new Mock<Name>("John Doe");

            var user = new User("john.doe@example.com", "Admin", mockPhoneNumber.Object, mockName.Object, mockLastName.Object, mockFullName.Object);
            var newFirstName = new Mock<Name>("Jane");
            var newLastName = new Mock<Name>("Smith");

            user.UpdateName(newFirstName.Object, newLastName.Object);

            Assert.Equal("Jane", user.FirstName.NameValue);
            Assert.Equal("Smith", user.LastName.NameValue);
        }
    }
}
