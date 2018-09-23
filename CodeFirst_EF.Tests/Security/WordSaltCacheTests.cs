using CodeFirst_EF.DTOs;
using CodeFirst_EF.Repositories;
using CodeFirst_EF.Security;
using Moq;
using NUnit.Framework;
using TddXt.AnyRoot;
using TddXt.AnyRoot.Numbers;
using TddXt.AnyRoot.Strings;

namespace CodeFirst_EF.Tests.Security
{
    [TestFixture]
    public class WordSaltCacheTests
    {
        [Test]
        public void GivenTheCacheIsInitialisingThenTheRepositoryIsRead()
        {
            var mock = new Mock<IRepository>();
            var sut = new WordSaltCache();

            sut.Init(mock.Object);

            mock.Verify(r => r.Get<WordMetric>(null, null, null), Times.Once);
        }

        [Test]
        public void GivenAKeyExistsInTheCacheAGetWillReturnTheSalt()
        {
            var key = Root.Any.String();
            var salt = Root.Any.String();

            var mock = new Mock<IRepository>();
            mock.Setup(r => r.Get<WordMetric>(null, null, null)).Returns(new[]
            {
                new WordMetric(Root.Any.String(), key, Root.Any.Integer(), salt)
            });

            var sut = new WordSaltCache();
            sut.Init(mock.Object);

            var result = sut.Get(key);

            Assert.AreEqual(salt, result);
        }

        [Test]
        public void AKeyCanBeAddedToTheCache()
        {
            var key = Root.Any.String();
            var salt = Root.Any.String();
            var mock = new Mock<IRepository>();
            var sut = new WordSaltCache();
            sut.Init(mock.Object);

            sut.Add(key, salt);

            Assert.AreEqual(salt, sut.Get(key));
        }
    }
}
