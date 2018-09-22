using CodeFirst_EF.Security;
using NUnit.Framework;
using TddXt.AnyRoot;
using TddXt.AnyRoot.Strings;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace CodeFirst_EF.Tests.Security
{
    [TestFixture]
    public class PBKDF2ProviderTests
    {
        private readonly PBKDF2Provider _sut = new PBKDF2Provider();

        [Test]
        public void GivenSameWordWithNoSaltWhenHashedTheHashIsDifferent()
        {
            var inputWord = Root.Any.String();

            var firstHashResult = _sut.CreateHash(inputWord);
            var secondHashResult = _sut.CreateHash(inputWord);

            Assert.AreNotEqual(firstHashResult.Hash, secondHashResult.Hash);
            Assert.AreNotEqual(firstHashResult.Salt, secondHashResult.Salt);
        }

        [Test]
        public void GivenSameWordAndSameSaltWhenHashedTheHashIsSame()
        {
            var inputWord = Root.Any.String();

            var firstHashResult = _sut.CreateHash(inputWord);
            var secondHashResult = _sut.CreateHash(inputWord, firstHashResult.Salt);

            Assert.AreEqual(firstHashResult.Hash, secondHashResult.Hash);
            Assert.AreEqual(firstHashResult.Salt, secondHashResult.Salt);
        }

        [Test]
        public void GivenDifferentWordsAndSameSaltWhenHashedTheHashIsDifferent()
        {
            var firstInputWord = Root.Any.String();
            var secondInputWord = Root.Any.String();

            var firstHashResult = _sut.CreateHash(firstInputWord);
            var secondHashResult = _sut.CreateHash(secondInputWord, firstHashResult.Salt);

            Assert.AreNotEqual(firstHashResult.Hash, secondHashResult.Hash);
            Assert.AreEqual(firstHashResult.Salt, secondHashResult.Salt);
        }
    }
}
