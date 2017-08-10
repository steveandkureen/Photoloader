using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.OAuth.Messages;
using Newtonsoft.Json;
using NLog;
using SmugMugWrapper.V2;

namespace SmugMugWrapper
{
    public class TokenManager : IConsumerTokenManager
    {
        public TokenManager(string token, string tokenSecret)
        {
            Token = token;
            TokenSecret = tokenSecret;
        }

        public string GetTokenSecret(string token)
        {
            return TokenSecret;
        }

        public void StoreNewRequestToken(UnauthorizedTokenRequest request, ITokenSecretContainingMessage response)
        {
            Token = response.Token;
            TokenSecret = response.TokenSecret;
        }

        public void ExpireRequestTokenAndStoreNewAccessToken(string consumerKey, string requestToken, string accessToken,
            string accessTokenSecret)
        {
            Token = accessToken;
            TokenSecret = accessTokenSecret;
        }

        public TokenType GetTokenType(string token)
        {
            return TokenType.AccessToken;
        }

        public string ConsumerKey => SmugHelper.ApiKey;
        public string ConsumerSecret => SmugHelper.SecretApiKey;

        public string Token { get; set; }
        public string TokenSecret { get; set; }
    }


    public class TreeNode
    {
        public TreeNode()
        {
            Children = new List<TreeNode>();
        }

        public string Name { get; set; }
        public string Id { get; set; }
        public string Uri {get; set; }
        public string AlbumUri { get; set; }
        public List<TreeNode> Children { get; set; }
    }

    public class SmugMugApiV2 
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private OAuthToken oAuthToken;
        private static string ChildNodeSuffix = "!children?count=1000";
        private DesktopConsumer consumer;

        public TreeNode RootNode { get; set; }


        public SmugMugApiV2(OAuthToken token) 
        {
            var tokenManager = new TokenManager(token.Token, token.TokenSecret);
            var serviceProvider = new ServiceProviderDescription()
            {
                ProtocolVersion = ProtocolVersion.V10a,
                AccessTokenEndpoint = new MessageReceivingEndpoint(SmugHelper.AccessUrl, HttpDeliveryMethods.GetRequest),
                RequestTokenEndpoint = new MessageReceivingEndpoint(SmugHelper.RequestUrl, HttpDeliveryMethods.GetRequest),
                UserAuthorizationEndpoint =
                new MessageReceivingEndpoint(SmugHelper.AuthorizeUrl, HttpDeliveryMethods.GetRequest),
                TamperProtectionElements = new ITamperProtectionChannelBindingElement[] {
                    new HmacSha1SigningBindingElement(),
                },

            };

            oAuthToken = token;
            consumer = new DesktopConsumer(serviceProvider, tokenManager);
        }

        #region Find Nodes
        public TreeNode FindYear(string searchString)
        {
            if (RootNode == null)
            {
                FindRootNode();
            }

            if (RootNode.Children.Count == 0)
            {
                LoadChildNodes(RootNode);
            }

            return FindChildNode(RootNode, searchString);
        }

        public TreeNode FindMonth(TreeNode year, string searchString)
        {
            LoadChildNodes(year);
            return FindChildNode(year, searchString);
        }

        public TreeNode FindDay(TreeNode month, string searchString)
        {
            LoadChildNodes(month);
            return FindChildNode(month, searchString);
        }

        public TreeNode FindRootNode()
        {
            if (RootNode != null)
            {
                return RootNode;
            }

            var response = MakeGetRequest("/api/v2!authuser");
            response.EnsureSuccessStatusCode();
            var responseString = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<RootObject>(responseString);
            RootNode = new TreeNode
            {
                Name = result.Response.User.Uris.Node.UriDescription,
                Uri = result.Response.User.Uris.Node.Uri
            };
            return RootNode;
        }

        private HttpResponseMessage MakeGetRequest(string requestUri)
        {
            var client = new HttpClient(consumer.CreateAuthorizingHandler(oAuthToken.Token));
            client.BaseAddress = new Uri(SmugHelper.ApiBaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var responseTask = client.GetAsync(SmugHelper.ApiBaseUrl + requestUri);
            responseTask.Wait();

            var response = responseTask.Result;
            return response;
        }

        private HttpResponseMessage MakePost(string requestUri, string body)
        {
            var client = new HttpClient(consumer.CreateAuthorizingHandler(oAuthToken.Token));
            client.BaseAddress = new Uri(SmugHelper.ApiBaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var responseTask = client.PostAsync(SmugHelper.ApiBaseUrl + requestUri, new StringContent(body));
            responseTask.Wait();

            var response = responseTask.Result;
            return response;
        }


        private TreeNode FindChildNode(TreeNode parent, string searchString)
        {
            return parent.Children.SingleOrDefault(c => c.Name == searchString);
        }

        private void LoadChildNodes(TreeNode parent)
        {
            // check to see if children are already loaded
            if (parent.Children.Count != 0)
                return;

            var response = MakeGetRequest(parent.Uri + ChildNodeSuffix);
            response.EnsureSuccessStatusCode();
            var responseString = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<RootObject>(responseString);

            // no children
            if (result.Response.Node == null)
                return;

            foreach (var node in result.Response.Node)
            {
                var newNode = CreateTreeNode(node);
                parent.Children.Add(newNode);
            }
        }

        private static TreeNode CreateTreeNode(Node node)
        {
            var newNode = new TreeNode {Id = node.NodeID, Name = node.Name, Uri = node.Uri};
            try
            {
                if (node.Type == "Album")
                {
                    newNode.AlbumUri = node.Uris.Album.Uri;
                }
            }
            catch (Exception)
            {
                // Album Uri doesn't exist   
            }
            return newNode;
        }

        #endregion

#region Make Nodes

        public TreeNode AddNode(TreeNode parent, NewNode newNode)
        {
            var body = JsonConvert.SerializeObject(newNode);

            var response = MakePost(parent.Uri + ChildNodeSuffix, body);
            response.EnsureSuccessStatusCode();
            var responseString = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<RootAddObject>(responseString);
          
            var node = result.Response.Node;

            var resultNode = CreateTreeNode(node);
            parent.Children.Add(resultNode);
            return resultNode;
        }
        #endregion

        private static string GetStringFromHash(byte[] arr)
        {
            StringBuilder s = new StringBuilder();
            foreach (byte item in arr)
            {
                var first = item >> 4;
                var second = (item & 0x0F);

                s.AppendFormat("{0:X}{1:X}", first, second);
            }

            return s.ToString().ToLower();
        }

        public void UploadImage(string imagePath, TreeNode album)
        {
            logger.Info($"Uploading Image: {imagePath} to album: {album.Name}");

            var byteArr = File.ReadAllBytes(imagePath);
            var md5Sum = GetStringFromHash(System.Security.Cryptography.MD5.Create().ComputeHash(byteArr));


            var fileName = Path.GetFileName(imagePath);
            var url = SmugHelper.UploadUrl;
            var headers = new Dictionary<string, string>
                          {
                             {"X-Smug-AlbumUri", album.AlbumUri},
                             {"X-Smug-Version", "2.0"},
                             {"X-Smug-FileName", fileName},
                             {"X-Smug-ResponseType", "JSON"},
                             {"Content-MD5", md5Sum }
                          };
            var fileInfo = new FileInfo(imagePath);
            using (var client = new HttpClient(consumer.CreateAuthorizingHandler(oAuthToken.Token)))
            {
                using (var content = new StreamContent(File.OpenRead(imagePath)))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("binary/octet-stream");
                    foreach (var key in headers.Keys)
                    {
                        content.Headers.Add(key, headers[key]);
                    }
                    content.Headers.ContentLength = fileInfo.Length;
                    using (var response = client.PostAsync(url, content).Result)
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            var contentString = response.Content.ReadAsStringAsync().Result;
                            throw new ApplicationException($"Image filed to upload with code: {response.StatusCode} {response.ReasonPhrase} \n\t\t {contentString}");
                        }
                        else
                        {
                            dynamic data = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
                            if (data.stat != "ok")
                            {
                                throw new ApplicationException("Image filed to upload");
                            }
                        }
                    }
                }
            }
        }

        public OAuthToken GetAccessToken()
        {
            var requestToken = string.Empty;
            var uri = consumer.RequestUserAuthorization(new Dictionary<string, string>(), new Dictionary<string, string>(), out requestToken);
            var browserWindow = new AuthenticationWindow(uri.AbsoluteUri);
            browserWindow.ShowDialog();

            var accessToken = consumer.ProcessUserAuthorization(requestToken, null);
            
            if (accessToken == null)
            {
                throw new InvalidOperationException("Token is null");
            }

            var tokenManger = consumer.TokenManager as TokenManager;

            return new OAuthToken {Token = tokenManger.Token, TokenSecret = tokenManger.TokenSecret};
        }
    }
}
