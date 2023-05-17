using System;
using Aspose.Tests.GoldComparers;
using NUnit.Framework;
using UserDocsApiLinker;


namespace UserDoscApiLinker
{
    /// <summary>
    /// Unit tests for UserDocsApiLinker
    /// </summary>
    [TestFixture]
    public class UnitTests
    {
        [Test]
        public void R11()
        {
            TestUtils.RunTest("R11", ApiMemberPlatform.Net);
        }

        [Test]
        public void R12()
        {
            TestUtils.RunTest("R12", ApiMemberPlatform.Net);
        }

        [Test]
        public void R13()
        {
            TestUtils.RunTest("R13", ApiMemberPlatform.Net);
        }

        [Test]
        public void R131()
        {
            TestUtils.RunTest("R131", ApiMemberPlatform.Net);
        }

        [Test]
        public void R132()
        {
            TestUtils.RunTest("R132", ApiMemberPlatform.Net);
        }

        [Test]
        public void R133()
        {
            TestUtils.RunTest("R133", ApiMemberPlatform.Net);
        }

        [Test]
        public void R134()
        {
            TestUtils.RunTest("R134", ApiMemberPlatform.Net);
        }

        [Test]
        public void R135()
        {
            TestUtils.RunTest("R135", ApiMemberPlatform.Net);
        }

        [Test, Ignore("Events no longer supported.")]
        public void R136()
        {
            TestUtils.RunTest("R136", ApiMemberPlatform.Net);
        }

        [Test]
        public void R211()
        {
            ApiMemberCollectionNet apiMemberCollection = new ApiMemberCollectionNet();
            apiMemberCollection.AddApiMember("T:Aspose.Words.DocumentVisitor");
            apiMemberCollection.AddApiMember("T:Aspose.Words.ParagraphAlignment");
            apiMemberCollection.AddApiMember("T:Aspose.Words.INodeCollection");
            Assert.AreEqual("DocumentVisitor", apiMemberCollection.ApiMembers[0].FriendlyName);
            Assert.AreEqual("ParagraphAlignment", apiMemberCollection.ApiMembers[1].FriendlyName);
            Assert.AreEqual("INodeCollection", apiMemberCollection.ApiMembers[2].FriendlyName);
        }

        [Test]
        public void R212()
        {
            ApiMemberCollectionNet apiMemberCollection = new ApiMemberCollectionNet();
            apiMemberCollection.AddApiMember("M:Aspose.Words.DocumentVisitor.VisitDocumentStart(Aspose.Words.Document)");
            apiMemberCollection.AddApiMember("M:Aspose.Words.Document.RemoveAllChildren");
            Assert.AreEqual("DocumentVisitor.VisitDocumentStart", apiMemberCollection.ApiMembers[0].FriendlyName);
            Assert.AreEqual("Document.RemoveAllChildren", apiMemberCollection.ApiMembers[1].FriendlyName);
        }

        [Test]
        public void R213()
        {
            ApiMemberCollectionNet apiMemberCollection = new ApiMemberCollectionNet();
            apiMemberCollection.AddApiMember("M:Aspose.Words.Document.#ctor");
            apiMemberCollection.AddApiMember("M:Aspose.Words.Document.#ctor(System.String)");
            Assert.AreEqual("Document", apiMemberCollection.ApiMembers[0].FriendlyName);
            Assert.AreEqual("Document", apiMemberCollection.ApiMembers[1].FriendlyName);
        }

        [Test]
        public void R214()
        {
            ApiMemberCollectionNet apiMemberCollection = new ApiMemberCollectionNet();
            apiMemberCollection.AddApiMember("P:Aspose.Words.Font.NameBi");
            apiMemberCollection.AddApiMember("P:Aspose.Words.TabStopCollection.Item(System.Int32)");
            Assert.AreEqual("Font.NameBi", apiMemberCollection.ApiMembers[0].FriendlyName);
            Assert.AreEqual("TabStopCollection.Item", apiMemberCollection.ApiMembers[1].FriendlyName);
        }

        [Test]
        public void R215()
        {
            ApiMemberCollectionNet apiMemberCollection = new ApiMemberCollectionNet();
            apiMemberCollection.AddApiMember("F:Aspose.Words.ParagraphAlignment.Left");
            Assert.AreEqual("ParagraphAlignment.Left", apiMemberCollection.ApiMembers[0].FriendlyName);
        }

        [Test, Ignore("Events no longer supported.")]
        public void R216()
        {
            ApiMemberCollectionNet apiMemberCollection = new ApiMemberCollectionNet();
            apiMemberCollection.AddApiMember("E:Aspose.Words.SaveOptions.HtmlExportImageSaving");
            Assert.AreEqual("SaveOptions.HtmlExportImageSaving", apiMemberCollection.ApiMembers[0].FriendlyName);
        }

        [Test]
        public void R221()
        {
            ApiMemberCollectionJava apiMemberCollection = new ApiMemberCollectionJava();
            apiMemberCollection.AddApiMember("T:Aspose.Words.DocumentVisitor");
            apiMemberCollection.AddApiMember("T:Aspose.Words.ParagraphAlignment");
            apiMemberCollection.AddApiMember("T:Aspose.Words.INodeCollection");
            Assert.AreEqual("DocumentVisitor", apiMemberCollection.ApiMembers[0].FriendlyName);
            Assert.AreEqual("ParagraphAlignment", apiMemberCollection.ApiMembers[1].FriendlyName);
            Assert.AreEqual("INodeCollection", apiMemberCollection.ApiMembers[2].FriendlyName);
        }

        [Test]
        public void R222()
        {
            ApiMemberCollectionJava apiMemberCollection = new ApiMemberCollectionJava();
            apiMemberCollection.AddApiMember("M:Aspose.Words.DocumentVisitor.VisitDocumentStart(Aspose.Words.Document)");
            apiMemberCollection.AddApiMember("M:Aspose.Words.Document.RemoveAllChildren");
            Assert.AreEqual("DocumentVisitor.visitDocumentStart", apiMemberCollection.ApiMembers[0].FriendlyName);
            Assert.AreEqual("Document.removeAllChildren", apiMemberCollection.ApiMembers[1].FriendlyName);
        }

        [Test]
        public void R223()
        {
            ApiMemberCollectionJava apiMemberCollection = new ApiMemberCollectionJava();
            apiMemberCollection.AddApiMember("M:Aspose.Words.Document.#ctor");
            apiMemberCollection.AddApiMember("M:Aspose.Words.Document.#ctor(System.String)");
            Assert.AreEqual("Document", apiMemberCollection.ApiMembers[0].FriendlyName);
            Assert.AreEqual("Document", apiMemberCollection.ApiMembers[1].FriendlyName);
        }

        [Test]
        public void R2241()
        {
            ApiMemberCollectionJava apiMemberCollection = new ApiMemberCollectionJava();
            apiMemberCollection.AddApiMember("P:Aspose.Words.Font.NameBi", false);
            Assert.AreEqual("Font.getNameBi/setNameBi", apiMemberCollection.ApiMembers[0].FriendlyName);
        }

        [Test]
        public void R2242()
        {
            ApiMemberCollectionJava apiMemberCollection = new ApiMemberCollectionJava();
            apiMemberCollection.AddApiMember("P:Aspose.Words.Font.Border", true);
            Assert.AreEqual("Font.getBorder", apiMemberCollection.ApiMembers[0].FriendlyName);
        }

        [Test]
        public void R2243()
        {
            ApiMemberCollectionJava apiMemberCollection = new ApiMemberCollectionJava();
            apiMemberCollection.AddApiMember("P:Aspose.Words.TabStopCollection.Item(System.Int32)", false);
            Assert.AreEqual("TabStopCollection.get/set", apiMemberCollection.ApiMembers[0].FriendlyName);
        }

        [Test]
        public void R2244()
        {
            ApiMemberCollectionJava apiMemberCollection = new ApiMemberCollectionJava();
            apiMemberCollection.AddApiMember("P:Aspose.Words.CompositeNode.HasChildNodes", true);
            Assert.AreEqual("CompositeNode.hasChildNodes", apiMemberCollection.ApiMembers[0].FriendlyName);
        }

        [Test]
        public void R2245()
        {
            ApiMemberCollectionJava apiMemberCollection = new ApiMemberCollectionJava();
            apiMemberCollection.AddApiMember("P:Aspose.Words.HeaderFooterCollection.Item(System.Int32)", true);
            apiMemberCollection.AddApiMember("P:Aspose.Words.HeaderFooterCollection.Item(Aspose.Words.HeaderFooterType)", true);
            Assert.AreEqual("HeaderFooterCollection.get", apiMemberCollection.ApiMembers[0].FriendlyName);
            Assert.AreEqual("HeaderFooterCollection.getByHeaderFooterType", apiMemberCollection.ApiMembers[1].FriendlyName);
        }

        [Test]
        public void R225()
        {
            ApiMemberCollectionJava apiMemberCollection = new ApiMemberCollectionJava();
            apiMemberCollection.AddApiMember("F:Aspose.Words.ParagraphAlignment.Left");
            apiMemberCollection.AddApiMember("F:Aspose.Words.BreakType.PageBreak");
            apiMemberCollection.AddApiMember("F:Aspose.Words.PaperSize.A4");
            apiMemberCollection.AddApiMember("F:Aspose.Words.PaperSize.Paper10x14");
            apiMemberCollection.AddApiMember("F:Aspose.Words.VisitorAction.SkipThisNode");
            Assert.AreEqual("ParagraphAlignment.LEFT", apiMemberCollection.ApiMembers[0].FriendlyName);
            Assert.AreEqual("BreakType.PAGE_BREAK", apiMemberCollection.ApiMembers[1].FriendlyName);
            Assert.AreEqual("PaperSize.A4", apiMemberCollection.ApiMembers[2].FriendlyName);
            Assert.AreEqual("PaperSize.PAPER_10_X_14", apiMemberCollection.ApiMembers[3].FriendlyName);
            Assert.AreEqual("VisitorAction.SKIP_THIS_NODE", apiMemberCollection.ApiMembers[4].FriendlyName); 
        }

        [Test]
        public void R226()
        {
            ApiMemberCollectionJava apiMemberCollection = new ApiMemberCollectionJava();
            try
            {
                apiMemberCollection.AddApiMember("E:Aspose.Words.SaveOptions.HtmlExportImageSaving");
            }
            catch (Exception ex)
            {

                Assert.AreEqual("There is incorrect input string.", ex.Message);
            }
        }

        [Test]
        public void R31()
        {
            TestUtils.RunTest("R31", ApiMemberPlatform.Net);
        }

        [Test]
        public void R32()
        {
            TestUtils.RunTest("R32", ApiMemberPlatform.Net);
        }

        [Test]
        public void R33()
        {
            string testTestData = TestUtils.BuildTestFileName("R33", "docx");
            string testGoldData = TestUtils.BuildGoldFileName("R33", "docx");
            string testOutData = TestUtils.BuildOutFileName("R33", "docx");

            ApiMemberLinker apiMemberLinker = new ApiMemberLinker(ApiMemberPlatform.Net, @"mk:@MSITStore:Test.chm::/", TestUtils.ReturnAssemblyFile(ApiMemberPlatform.Net), TestUtils.ReturnXmlFile(ApiMemberPlatform.Net), "log.txt", 3);
            apiMemberLinker.ProcessDocument(testTestData, testOutData);
            ZipFileComparer.Execute("R33", testTestData, testOutData, testGoldData, testOutData);  
        }

        [Test]
        public void R341()
        {
            try
            {
                TestUtils.RunTest("R341", ApiMemberPlatform.Net);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("There is no Api member with given name or parameters found - \"Aspose.Words.Document.Clone(System.String)\"", ex.Message);
            }
        }
        [Test]
        public void R342()
        {
            TestUtils.RunTest("R342", ApiMemberPlatform.Net);
        }
        [Test]
        public void R343()
        {
            TestUtils.RunTest("R343", ApiMemberPlatform.Net);
        }
        [Test]
        public void R3431()
        {
            TestUtils.RunTest("R3431", ApiMemberPlatform.Net);
        }
        [Test]
        public void R344()
        {
            TestUtils.RunTest("R344", ApiMemberPlatform.Net);
        }
        [Test]
        public void R345()
        {
            TestUtils.RunTest("R345", ApiMemberPlatform.Net);
        }
        [Test]
        public void R346()
        {
            TestUtils.RunTest("R346", ApiMemberPlatform.Net);
        }
        [Test, Ignore("Events no longer supported")]
        public void R347()
        {
            TestUtils.RunTest("R347", ApiMemberPlatform.Net);
        }

        [Test]
        public void DefectWhitespace()
        {
            TestUtils.RunTest("DefectWhitespace", ApiMemberPlatform.Net);
        }

        [Test]
        public void DefectC4()
        {
            TestUtils.RunTest("DefectC4", ApiMemberPlatform.Net);
        }

        [Test]
        public void MemberWithoutParameters()
        {
            TestUtils.RunTest("MemberWithoutParameters", ApiMemberPlatform.Net);
        }

        [Test]
        public void TestLogingContinueWorking()
        {
            string testTestData = TestUtils.BuildTestFileName("TestLogingContinueWorking", "docx");
            string testGoldData = TestUtils.BuildGoldFileName("TestLogingContinueWorking", "docx");
            string testOutData = TestUtils.BuildOutFileName("TestLogingContinueWorking", "docx");

            ApiMemberLinker apiMemberLinker = new ApiMemberLinker(ApiMemberPlatform.Net, TestUtils.ReturnUrl(), TestUtils.ReturnAssemblyFile(ApiMemberPlatform.Net), TestUtils.ReturnXmlFile(ApiMemberPlatform.Net), "log.txt", 2);
            apiMemberLinker.ProcessDocument(testTestData, testOutData);
            ZipFileComparer.Execute("TestLogingContinueWorking", testTestData, testOutData, testGoldData, testOutData);
        }

        [Test]
        public void Table()
        {
            TestUtils.RunTest("Table", ApiMemberPlatform.Net);
        }
        [Test]
        public void NoParamNoOverloads()
        {
            string testTestData = TestUtils.BuildTestFileName("NoParamNoOverloads", "docx");
            string testGoldData = TestUtils.BuildGoldFileName("NoParamNoOverloads", "docx");
            string testOutData = TestUtils.BuildOutFileName("NoParamNoOverloads", "docx");

            ApiMemberLinker apiMemberLinker = new ApiMemberLinker(ApiMemberPlatform.Net, TestUtils.ReturnUrl(), TestUtils.ReturnAssemblyFile(ApiMemberPlatform.Net), TestUtils.ReturnXmlFile(ApiMemberPlatform.Net), "log.txt", 2);
            apiMemberLinker.ProcessDocument(testTestData, testOutData);
            ZipFileComparer.Execute("NoParamNoOverloads", testTestData, testOutData, testGoldData, testOutData);
        }

        [Test]
        public void BaseType()
        {
            TestUtils.RunTest("BaseType", ApiMemberPlatform.Net);
        }
    }
}
