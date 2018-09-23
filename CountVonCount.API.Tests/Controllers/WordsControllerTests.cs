﻿using CodeFirst_EF.Repositories;
using CountVonCount.API.Controllers;
using Moq;
using NUnit.Framework;

namespace CountVonCount.API.Tests.Controllers
{
    [TestFixture]
    public class WordsControllerTests
    {
        [Test]
        public void WhenGetIsCalledRepositoryGetMethodIsCalledOnce()
        {
            var mockRepository = new Mock<IWordRepository>();
            var sut = new WordsController(mockRepository.Object);

            sut.Get();

            mockRepository.Verify(x => x.GetTop(100));
        }
    }
}
