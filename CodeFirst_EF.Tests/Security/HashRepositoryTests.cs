using System.Linq;
using CodeFirst_EF.DTOs;
using CodeFirst_EF.Security;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using TddXt.AnyRoot;
using TddXt.AnyRoot.Numbers;
using TddXt.AnyRoot.Strings;

namespace CodeFirst_EF.Tests.Security
{
    [TestFixture]
    public class HashRepositoryTests
    {
        [Test]
        public void GivenAnEmptySaltCacheHashWillGenerateSalt()
        {
            var mockSaltCache = new Mock<ISaltCache>();
            var savedSaltKey = "";
            var savedSaltValue = "";
            mockSaltCache.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<string>())).Callback<string, string>(
                (key, value) =>
                {
                    savedSaltKey = key;
                    savedSaltValue = value;
                });

            var mockHashProvider = new Mock<IHashProvider>();
            var hashResult = new HashResult(Root.Any.String(), Root.Any.String());
            mockHashProvider.Setup(h => h.CreateHash(It.IsAny<string>(), It.IsAny<string>())).Returns(hashResult);

            var sut = new HashRepository(mockHashProvider.Object, mockSaltCache.Object, true);

            const string unhashedId = "unhashed";
            var entity = new WordMetric(unhashedId, Root.Any.String(), Root.Any.Integer(), null);

            var hashedEntities = sut.Hash(new[] {entity});
            var hashedEntity = hashedEntities.First();

            Assert.IsFalse(unhashedId == hashedEntity.Id);
            Assert.AreEqual(hashResult.Hash, hashedEntity.Id);

            Assert.AreEqual(hashResult.Salt, hashedEntity.Salt);

            Assert.AreEqual(hashResult.Salt, savedSaltValue);
            Assert.AreEqual(unhashedId, savedSaltKey);
        }

        [Test]
        public void GivenWordWhichExistsInTheCacheThenHashWillReuseSalt()
        {
            var mockSaltCache = new Mock<ISaltCache>();
            var knownSalt = Root.Any.String();
            var knownWord = Root.Any.String();
            mockSaltCache.Setup(s => s.Get(knownWord)).Returns(knownSalt);

            var mockHashProvider = new Mock<IHashProvider>();
            var hashResult = new HashResult(Root.Any.String(), knownSalt);
            mockHashProvider.Setup(h => h.CreateHash(knownWord, knownSalt)).Returns(hashResult);

            var sut = new HashRepository(mockHashProvider.Object, mockSaltCache.Object, true);
            var entity = new WordMetric(knownWord, knownWord, Root.Any.Integer(), null);

            var hashedEntities = sut.Hash(new[] { entity });

            mockHashProvider.Verify(h => h.CreateHash(knownWord, knownSalt), Times.Once);
        }
    }
}
