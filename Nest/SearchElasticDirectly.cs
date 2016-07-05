using System;
using FluentAssertions;
using Moq;
using Nest;
using NUnit.Framework;

namespace NestTest
{
    [TestFixture]
    public class SearchElasticDirectly
    {
        [Test]
        public void MatchWithFuncReturnsExpected()
        {
            var expectedSearchDescriptor = new SearchDescriptor<string>().Query(q => q.Ids(new[] { "jemima" }));
            Func<SearchDescriptor<string>, SearchDescriptor<string>> searchFunc = 
                s=> s.Query(q => q.Ids(new[] { "jemima" }));

            var mockResponse = new Mock<ISearchResponse<string>>();
            mockResponse.Setup(x => x.Documents).Returns(new[] { "left", "right" });

            var thingy = new Mock<IElasticClient>();
            var serializedSearchDescriptor = expectedSearchDescriptor.Serialize();
            thingy.Setup(x => x.Search<string>(It.Is<SearchDescriptor<string>>(sd => sd.Serialize() == serializedSearchDescriptor)))
                .Returns(mockResponse.Object);

            var result = thingy.Object.Search<string>(searchFunc(new SearchDescriptor<string>()));
            result.Documents.Should().HaveCount(2);
        }

        [Test]
        public void MatchReturnsExpected()
        {
            var expectedSearchDescriptor = new SearchDescriptor<string>().Query(q => q.Ids(new[] { "jemima" }));
            var duplicate = new SearchDescriptor<string>().Query(q => q.Ids(new[] { "jemima" }));

            var mockResponse = new Mock<ISearchResponse<string>>();
            mockResponse.Setup(x => x.Documents).Returns(new [] {"left", "right"});

            var thingy = new Mock<IElasticClient>();
            var serializedSearchDescriptor = expectedSearchDescriptor.Serialize();
            thingy.Setup(x => x.Search<string>(It.Is<SearchDescriptor<string>>(sd => sd.Serialize() == serializedSearchDescriptor)))
                .Returns(mockResponse.Object);

            var result = thingy.Object.Search<string>(duplicate);
            result.Documents.Should().HaveCount(2);
        }

        [Test]
        public void UnmatchedReturnsNull()
        {
            var orig = new SearchDescriptor<string>().Query(q => q.Ids(new[] { "jemima" }));
            var different = new SearchDescriptor<string>().Query(q => q.Ids(new[] { "jemima", "puddleduck" }));

            var mockResponse = new Mock<ISearchResponse<string>>();
            mockResponse.Setup(x => x.Documents).Returns(new[] { "left", "right" });

            var thingy = new Mock<IElasticClient>();
            var serializedSearchDescriptor = orig.Serialize();
            thingy.Setup(x => x.Search<string>(It.Is<SearchDescriptor<string>>(sd => sd.Serialize() == serializedSearchDescriptor)))
                .Returns(mockResponse.Object);

            var result = thingy.Object.Search<string>(different);
            result.Should().BeNull();
        }
    }
}