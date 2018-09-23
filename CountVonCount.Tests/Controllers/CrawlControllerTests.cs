using CodeFirst_EF.Collectors;
using CodeFirst_EF.DTOs;
using CodeFirst_EF.Repositories;
using CountVonCount.Controllers;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace CountVonCount.Tests.Controllers
{
    [TestFixture]
    public class CrawlControllerTests
    {
        [Test]
        public void GivenPostIsCalledThenCollectAndSaveAreCalled()
        {
            var url = "https://www.bbc.co.uk/news/uk-politics-45621354";
            var collectMock = new Mock<ICollector>();
            collectMock.Setup(c => c.Collect(url)).Returns(new List<WordMetric>());
            var repositoryMock = new Mock<IWordRepository>();

            var sut = new CrawlController(collectMock.Object, repositoryMock.Object);
            sut.Post(new FormData {url = url});

            collectMock.Verify(c => c.Collect(url), Times.Once);
            repositoryMock.Verify(r => r.Save(It.IsAny<List<WordMetric>>()), Times.Once);
        }
    }
}
