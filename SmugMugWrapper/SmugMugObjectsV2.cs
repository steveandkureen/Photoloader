using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmugMugWrapper.V2
{
    public class Request
    {
        public string Version { get; set; }
        public string Method { get; set; }
        public string Uri { get; set; }
    }

    public class OPTIONS2
    {
        public List<string> Permissions { get; set; }
    }

    public class GET
    {
        public List<string> Permissions { get; set; }
    }

    public class MethodDetails
    {
        public OPTIONS2 OPTIONS { get; set; }
        public GET GET { get; set; }
    }

    public class ParameterDescription
    {
        public string Varchar { get; set; }
        public string Select { get; set; }
        public string Uri { get; set; }
    }

    public class GET2
    {
        public string Name { get; set; }
        public bool Required { get; set; }
        public bool ReadOnly { get; set; }
        public string Default { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int MIN_CHARS { get; set; }
        public string MAX_CHARS { get; set; }
        public string Value { get; set; }
        public List<string> OPTIONS { get; set; }
        public int? MIN_COUNT { get; set; }
        public object MAX_COUNT { get; set; }
        public string MAX_LENGTH { get; set; }
        public object Locator { get; set; }
    }

    public class Parameters
    {
        public List<GET2> GET { get; set; }
    }

    public class Options
    {
        public List<string> MediaTypes { get; set; }
        public MethodDetails MethodDetails { get; set; }
        public List<string> Methods { get; set; }
        public ParameterDescription ParameterDescription { get; set; }
        public Parameters Parameters { get; set; }
        public List<string> Notes { get; set; }
    }

    public class AlbumShareUris : NodeBase
    {
       
    }

    public class NodeBase
    {
        public string Uri { get; set; }
        public string UriDescription { get; set; }
        public string EndpointType { get; set; }
        public string Locator { get; set; }
        public string LocatorType { get; set; }
    }

    public class NewNode
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string UrlName => Name.Replace(" ", "-").Replace(",", "");
        public string Privacy => "Public";
    }

    public class Node : NodeBase
    {
        public string ResponseLevel { get; set; }
        public string WebUri { get; set; }
        public string Description { get; set; }
        public bool HideOwner { get; set; }
        public string Name { get; set; }
        public List<object> Keywords { get; set; }
        public string PasswordHint { get; set; }
        public string Privacy { get; set; }
        public string SecurityType { get; set; }
        public bool ShowCoverImage { get; set; }
        public string SmugSearchable { get; set; }
        public string SortDirection { get; set; }
        public string SortMethod { get; set; }
        public string Type { get; set; }
        public string UrlName { get; set; }
        public string WorldSearchable { get; set; }
        public string DateAdded { get; set; }
        public string DateModified { get; set; }
        public string EffectivePrivacy { get; set; }
        public string EffectiveSecurityType { get; set; }
        public FormattedValues FormattedValues { get; set; }
        public bool HasChildren { get; set; }
        public bool IsRoot { get; set; }
        public string NodeID { get; set; }
        public int SortIndex { get; set; }
        public string UrlPath { get; set; }
        public Uris Uris { get; set; }
    }

    public class FormattedValues
    {
        public Name Name { get; set; }
        public Description Description { get; set; }
    }

    public class Name
    {
        public string html { get; set; }
    }

    public class Description
    {
        public string html { get; set; }
        public string text { get; set; }
    }

    public class NodeCoverImage : NodeBase
    {
       
    }

    public class User : NodeBase
    {
        public string ResponseLevel { get; set; }
        public string WebUri { get; set; }
        public string AccountStatus { get; set; }
        public string FirstName { get; set; }
        public bool FriendsView { get; set; }
        public int ImageCount { get; set; }
        public bool IsTrial { get; set; }
        public string LastName { get; set; }
        public string NickName { get; set; }
        public string SortBy { get; set; }
        public string ViewPassHint { get; set; }
        public string ViewPassword { get; set; }
        public string Domain { get; set; }
        public string DomainOnly { get; set; }
        public string RefTag { get; set; }
        public string Name { get; set; }
        public string Plan { get; set; }
        public bool QuickShare { get; set; }
        public Uris Uris { get; set; }
    }

    public class Folder : NodeBase
    {
    
    }

    public class ParentFolders : NodeBase
    {
       
    }

    public class HighlightImage : NodeBase
    {
   
    }

    public class AlbumHighlightImage : NodeBase
    {
      
    }

    public class AlbumImages : NodeBase
    {
     
    }

    public class AlbumPopularMedia : NodeBase
    {
     
    }

    public class AlbumGeoMedia : NodeBase
    {
       
    }

    public class AlbumComments : NodeBase
    {
  
    }

    public class AlbumDownload : NodeBase
    {
   
    }

    public class AlbumPrices : NodeBase
    {
     
    }

    public class Album : NodeBase
    {
       
    }

    public class Uris
    {
        public AlbumShareUris AlbumShareUris { get; set; }
        public Node Node { get; set; }
        public NodeCoverImage NodeCoverImage { get; set; }
        public User User { get; set; }
        public Folder Folder { get; set; }
        public ParentFolders ParentFolders { get; set; }
        public HighlightImage HighlightImage { get; set; }
        public AlbumHighlightImage AlbumHighlightImage { get; set; }
        public AlbumImages AlbumImages { get; set; }
        public AlbumPopularMedia AlbumPopularMedia { get; set; }
        public AlbumGeoMedia AlbumGeoMedia { get; set; }
        public AlbumComments AlbumComments { get; set; }
        public AlbumDownload AlbumDownload { get; set; }
        public AlbumPrices AlbumPrices { get; set; }
        public Album Album { get; set; }
    }

    public class AlbumSearchResult
    {
        public string ResponseLevel { get; set; }
        public string Uri { get; set; }
        public string WebUri { get; set; }
        public string UriDescription { get; set; }
        public string NiceName { get; set; }
        public string UrlName { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public bool AllowDownloads { get; set; }
        public string Description { get; set; }
        public bool External { get; set; }
        public string Keywords { get; set; }
        public string PasswordHint { get; set; }
        public bool Protected { get; set; }
        public string SortDirection { get; set; }
        public string SortMethod { get; set; }
        public string SecurityType { get; set; }
        public string AlbumKey { get; set; }
        public string LastUpdated { get; set; }
        public string ImagesLastUpdated { get; set; }
        public string NodeID { get; set; }
        public int ImageCount { get; set; }
        public string UrlPath { get; set; }
        public bool CanShare { get; set; }
        public Uris Uris { get; set; }
    }

    public class Pages
    {
        public int Total { get; set; }
        public int Start { get; set; }
        public int Count { get; set; }
        public int RequestedCount { get; set; }
        public string FirstPage { get; set; }
        public string NextPage { get; set; }
        public string LastPage { get; set; }
    }

    public class Total
    {
        public double time { get; set; }
        public int cycles { get; set; }
        public int objects { get; set; }
    }

    public class Timing
    {
        public Total Total { get; set; }
    }

    public class Response : NodeBase
    {
        public List<Node> Node { get; set; }
        public User User { get; set; }
        public Pages Pages { get; set; }
        public Timing Timing { get; set; }
    }

    public class RootObject
    {
        public Request Request { get; set; }
        public Options Options { get; set; }
        public Response Response { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
    }

    public class ResponseAdd : NodeBase
    {
        public Node Node { get; set; }
        public User User { get; set; }
        public Pages Pages { get; set; }
        public Timing Timing { get; set; }
    }

    public class RootAddObject
    {
        public Request Request { get; set; }
        public Options Options { get; set; }
        public ResponseAdd Response { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
    }

    public class BioImage: NodeBase
    {
      
    }

    public class CoverImage : NodeBase
    {
       
    }

    public class UserProfile : NodeBase
    {
       
    }

    public class UserAlbums : NodeBase
    {
       
    }

    public class UserGeoMedia : NodeBase
    {
      
    }

    public class UserPopularMedia : NodeBase
    {
       
    }

    public class UserFeaturedAlbums : NodeBase
    {
       
    }

    public class UserRecentImages : NodeBase
    {
       
    }

    public class UserImageSearch : NodeBase
    {
      
    }

    public class UserTopKeywords : NodeBase
    {
        
    }

    public class UrlPathLookup : NodeBase
    {
      
    }

    public class UserAlbumTemplates : NodeBase
    {
      
    }

    public class SortUserFeaturedAlbums : NodeBase
    { 
    }

    public class UserTasks : NodeBase
    {
     
    }

    public class UserWatermarks : NodeBase
    {
     
    }

    public class UserPrintmarks : NodeBase
    {
       
    }

    public class UserUploadLimits : NodeBase
    {
     
    }

    public class UserAssetsAlbum : NodeBase
    {
      
    }

    public class UserLatestQuickNews : NodeBase
    {
     
    }

    public class UserGuideStates : NodeBase
    {
       
    }

    public class UserHideGuides : NodeBase
    {
      
    }

    public class Features : NodeBase
    {

    }

    public class UserGrants : NodeBase
    {

    }

    public class DuplicateImageSearch : NodeBase
    {

    }

    public class UserDeletedAlbums : NodeBase
    {

    }

    public class UserDeletedFolders : NodeBase
    {

    }

    public class UserDeletedPages : NodeBase
    {

    }

    public class UserContacts : NodeBase
    {

    }


}
