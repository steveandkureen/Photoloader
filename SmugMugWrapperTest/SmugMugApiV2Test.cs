using DotNetOpenAuth.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmugMugWrapper;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;
using SmugMugWrapper.V2;

namespace SmugMugWrapperTest
{
    [TestClass]
    public class SmugMugApiV2Test
    {
        private string tokenSecret = "122797437e5e46ebea2075ec9485fb8149cbbf405e02ccf0046cc698b323e987";
        private string token = "271a0ab373b043cd923bc76e9d317e70";
        private OAuthToken oAuthToken;
        private TokenManager tokenManager;
        private ServiceProviderDescription serviceProvider;

        [TestInitialize]
        public void InitTest()
        {
        }

        [TestMethod]
        public void RootNodeTest()
        {
            var api = new SmugMugApiV2(new OAuthToken() {Token = token, TokenSecret = tokenSecret});
            var results = api.FindRootNode();
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void FindYearTest()
        {
            var api = new SmugMugApiV2( new OAuthToken() { Token = token, TokenSecret = tokenSecret });
            var results = api.FindYear("2016");
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void DontFindYearTest()
        {
            var api = new SmugMugApiV2(new OAuthToken() { Token = token, TokenSecret = tokenSecret });
            var results = api.FindYear("1977");
            Assert.IsNull(results);
        }

        [TestMethod]
        public void LoadMonthTest()
        {
            var api = new SmugMugApiV2( new OAuthToken() { Token = token, TokenSecret = tokenSecret });
            var year = api.FindYear("2016");
            var month = api.FindMonth(year, "August");
            Assert.IsNotNull(month);
        }

        [TestMethod]
        public void LoadDayTest()
        {
            var api = new SmugMugApiV2( new OAuthToken() { Token = token, TokenSecret = tokenSecret });
            var year = api.FindYear("2016");
            var month = api.FindMonth(year, "August");
            var day = api.FindDay(month, "August 09, 2016");
            Assert.IsNotNull(day);
        }

        [TestMethod]
        public void AddTestFolder()
        {
            var api = new SmugMugApiV2( new OAuthToken() { Token = token, TokenSecret = tokenSecret });
            api.FindRootNode();
            //var testNode = api.AddNode(api.RootNode, new NewNode() {Name = "Test Api Node", Type = "Folder"});
            //Assert.IsNotNull(testNode);
        }

        [TestMethod]
        public void AddAlbumTest()
        {
            var api = new SmugMugApiV2(new OAuthToken() { Token = token, TokenSecret = tokenSecret });
            api.FindRootNode();
            var testNode = api.AddNode(api.RootNode, new NewNode() {Name = "Test Api Album", Type = "Album"});
            Assert.IsNotNull(testNode);
        }

        [TestMethod]
        public void TestUploadImage()
        {
            var api = new SmugMugApiV2(new OAuthToken() { Token = token, TokenSecret = tokenSecret });
            api.FindRootNode();
            var testNode = api.FindYear("Image Add Test Node");
            //var testNode = api.AddNode(api.RootNode, new NewNode() {Name = "Image Add Test Node", Type = "Album"});
            api.UploadImage(@"C:\Users\sschwarz\Pictures\Aiden.jpg", testNode);
        }
    }
}
