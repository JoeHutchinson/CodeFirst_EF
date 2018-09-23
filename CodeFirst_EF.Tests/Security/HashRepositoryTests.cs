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
        public void GivenAnEmptySaltCacheHashWillGenerateSalt()
        {
            var mockHashProvider = new Mock<IHashProvider>();
            var hashResult = new HashResult(Root.Any.String(), Root.Any.String());
            mockHashProvider.Setup(h => h.CreateHash(It.IsAny<string>(), It.IsAny<string>())).Returns(hashResult);

            var mockSaltCache = new Mock<ISaltCache>();
            var savedSaltKey = "";
            var savedSaltValue = "";
            mockSaltCache.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<string>())).Callback<string, string>(
                (key, value) =>
                {
                    savedSaltKey = key;
                    savedSaltValue = value;
                });

            var sut = new HashRepository(mockHashProvider.Object, mockSaltCache.Object);

            var entity = new WordMetric(Root.Any.String(), Root.Any.String(), Root.Any.Integer(), null);

            var hashedEntities = sut.Hash(new[] {entity});
            var hashedEntity = hashedEntities.First();

            Assert.AreNotEqual(entity.Id, hashedEntity.Id);
            Assert.AreEqual(hashResult.Hash, hashedEntity.Id);

            Assert.AreEqual(hashResult.Salt, hashedEntity.Salt);

            Assert.AreEqual(hashResult.Salt, savedSaltValue);
            Assert.AreEqual(hashedEntity.Word, savedSaltKey);
        }
    }
}
