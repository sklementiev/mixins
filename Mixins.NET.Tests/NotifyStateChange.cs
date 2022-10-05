namespace Mixins.Tests
{
    [TestFixture]
    public class NotifyStateChange
    {
        private Person person;

        [SetUp]
        public void Init()
        {
            person = new Person
            {
                FirstName = "Bob",
                LastName = "Minion",
                DateOfBirth = DateTime.Parse("11/11/11"),
            };
        }

        [Test]
        public void TestMNotifyStateChange()
        {
            var firstNameChanged = false;
            person.OnPropertyChanged(c => c.FirstName, s =>
            {
                Assert.AreEqual("New", s);
                firstNameChanged = true;
            });

            person.FirstName = "New";
            Assert.IsTrue(firstNameChanged);
        }

        [Test]
        public void AssigningSameValueDoesntChangeState()
        {
            var firstNameChanged = false;
            person.OnPropertyChanged(c => c.FirstName, s => { firstNameChanged = true; });

            person.FirstName = person.FirstName;
            Assert.IsFalse(firstNameChanged);
        }

        [Test]
        public void FullNameDependsOnOtherProperties()
        {
            var fullNameChanged = false;
            person.OnPropertyChanged(c => c.FullName, s => { fullNameChanged = true; });
            person.FirstName = "New";
            Assert.IsTrue(fullNameChanged);
            fullNameChanged = false;
            person.LastName = "New";
            Assert.IsTrue(fullNameChanged);
        }
    }
}