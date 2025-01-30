using System;

namespace SmugMugWrapper
{
   public static class SmugHelper
   {
      public static string ApiKey { get { return "{apiKye}"; } }
      public static string SecretApiKey { get { return "{apiSecret}"; } }

      public static string ApiBaseUrl { get { return "https://api.smugmug.com"; } }
      public static string V1ApiSecureBaseUrl { get { return "https://secure.smugmug.com/services/api/json/1.3.0/"; } }
      public static string RequestUrl { get { return "http://api.smugmug.com/services/oauth/getRequestToken.mg"; } }
      public static string AuthorizeUrl { get { return "http://api.smugmug.com/services/oauth/authorize.mg?oauth_token={0}&Access=Full&&Permissions=Modify"; } }
      public static string AccessUrl { get { return "http://api.smugmug.com/services/oauth/getAccessToken.mg"; } }

      public static string UploadUrl {get { return "http://upload.smugmug.com/"; }}
   }
}
