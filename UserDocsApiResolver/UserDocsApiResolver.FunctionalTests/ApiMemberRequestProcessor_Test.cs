using NUnit.Framework;
using UserDocsApiResolver.Library;
using UserDocsApiResolver.Library.ApiMemberStorage;

namespace UserDocsApiResolver.FunctionalTests
{
    [TestFixture]
    public class ApiMemberRequestProcessor_Test
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            ApiMemberStorageConfig config = new ApiMemberStorageConfigBuilder().Build();
            ApiMemberStorageManager storageManager = ApiMemberStorageManager.Create(config);
            _apiMemberRequestProcessor = new ApiMemberRequestProcessor(storageManager);
        }

        [Test]
        public void NetApiMemberRequest()
        {
            Assert.AreEqual("[Node.GetText|http://aspose.words/net/aspose.words.node.gettext.html]",
                            _apiMemberRequestProcessor.Process("Node.GetText", ApiMemberPlatform.Net));
            Assert.AreEqual("[Node.getText|http://aspose.words/net/aspose.words.node.gettext.html]",
                            _apiMemberRequestProcessor.Process("Node.getText", ApiMemberPlatform.Net));
            Assert.AreEqual("[Node.NodeType|http://aspose.words/net/aspose.words.node.nodetype.html]",
                            _apiMemberRequestProcessor.Process("Node.NodeType", ApiMemberPlatform.Net));
            Assert.AreEqual("*Node.getNodeType*",
                            _apiMemberRequestProcessor.Process("Node.getNodeType", ApiMemberPlatform.Net));
            Assert.AreEqual("[Document.Clone|http://aspose.words/net/aspose.words.document.clone_overload_2.html]",
                            _apiMemberRequestProcessor.Process("Document.Clone", ApiMemberPlatform.Net));
            Assert.AreEqual("*Document.deepClone*",
                            _apiMemberRequestProcessor.Process("Document.deepClone", ApiMemberPlatform.Net));
            Assert.AreEqual("*DataSet*", _apiMemberRequestProcessor.Process("DataSet", ApiMemberPlatform.Net));
            Assert.AreEqual("*DataSet.getTables*",
                            _apiMemberRequestProcessor.Process("DataSet.getTables", ApiMemberPlatform.Net));
            Assert.AreEqual("*DataSet.Tables*",
                            _apiMemberRequestProcessor.Process("DataSet.Tables", ApiMemberPlatform.Net));
            Assert.AreEqual("*SomeOtherClass.SomeOtherMethod*",
                            _apiMemberRequestProcessor.Process("SomeOtherClass.SomeOtherMethod", ApiMemberPlatform.Net));
        }

        [Test]
        public void JavaApiMemberRequest()
        {
            Assert.AreEqual("[Node.GetText|http://aspose.words/java/Node.html#getText()]",
                            _apiMemberRequestProcessor.Process("Node.GetText", ApiMemberPlatform.Java));
            Assert.AreEqual("[Node.getText|http://aspose.words/java/Node.html#getText()]",
                            _apiMemberRequestProcessor.Process("Node.getText", ApiMemberPlatform.Java));
            Assert.AreEqual("[Node.NodeType|http://aspose.words/java/Node.html#NodeType]",
                            _apiMemberRequestProcessor.Process("Node.NodeType", ApiMemberPlatform.Java));
            Assert.AreEqual("[Node.getNodeType|http://aspose.words/java/Node.html#NodeType]",
                            _apiMemberRequestProcessor.Process("Node.getNodeType", ApiMemberPlatform.Java));
            Assert.AreEqual("[Document.Clone|http://aspose.words/java/Document.html#deepClone()]",
                            _apiMemberRequestProcessor.Process("Document.Clone", ApiMemberPlatform.Java));
            Assert.AreEqual("[Document.deepClone|http://aspose.words/java/Document.html#deepClone()]",
                            _apiMemberRequestProcessor.Process("Document.deepClone", ApiMemberPlatform.Java));
            Assert.AreEqual("[DataSet|http://aspose.words/java/dataset.html]",
                            _apiMemberRequestProcessor.Process("DataSet", ApiMemberPlatform.Java));
            Assert.AreEqual("[DataSet.getTables|http://aspose.words/java/DataSet.html#getTables()]",
                            _apiMemberRequestProcessor.Process("DataSet.getTables", ApiMemberPlatform.Java));
            Assert.AreEqual("*DataSet.Tables*",
                            _apiMemberRequestProcessor.Process("DataSet.Tables", ApiMemberPlatform.Java));
            Assert.AreEqual("*SomeOtherClass.SomeOtherMethod*",
                            _apiMemberRequestProcessor.Process("SomeOtherClass.SomeOtherMethod", ApiMemberPlatform.Java));
        }

        private ApiMemberRequestProcessor _apiMemberRequestProcessor;
    }
}
