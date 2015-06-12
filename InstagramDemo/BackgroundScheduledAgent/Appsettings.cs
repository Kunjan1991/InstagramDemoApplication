using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnesocialBackgroundAgent
{
    public class LinkedInAppsettings
    {        
        public static string RequestTokenUri = "https://api.linkedin.com/uas/oauth/requestToken";
        public static string AuthorizeUri = "https://www.linkedin.com/uas/oauth/authorize";
        public static string AccessTokenUri = "https://api.linkedin.com/uas/oauth2/accessToken";
        public static string CallbackUri ="http://www.google.com";
        public static string consumerKey = "75bqpjrfzryil7"; 
        public static string consumerKeySecret ="edvUjwubGdIHmYjw"; 
        public static string oAuthVersion = "1.0";
        private const string ReauthenticateUrl = "https://www.linkedin.com/uas/oauth/authorize/oob";      
    }

    public class TumblrAppSettings
    {
        public static string RequestTokenUri = "http://www.tumblr.com/oauth/request_token";
        public static string AuthorizeUri = "http://www.tumblr.com/oauth/authorize";
        public static string AccessTokenUri = "http://www.tumblr.com/oauth/access_token";
        public static string CallbackUri = "http://www.google.com";//"tumblr://authorized";//"http://www.oauthtumblr.com";
        public static string consumerKey =  "zS0Q8mczAoDz589QXujNHa66XoCZYLR8q5XnYaDAHRIdjnXHRQ";// "zS0Q8mczAoDz589QXujNHa66XoCZYLR8q5XnYaDAHRIdjnXHRQ";//"75tx3ng6l8ony8";  "5z6tJRNYRLXThOOJ9emzD1VQUFY0MCAeaL9c4aWuG4hU5cqZq8";//
        public static string consumerKeySecret = "jbfvIix4LTjAP3r3isWRqiT9DQcH0TWv8Qw4b8BhN1cRwgiQot";//"jbfvIix4LTjAP3r3isWRqiT9DQcH0TWv8Qw4b8BhN1cRwgiQot";//"ZNTKdsoDwdNOh78q"; "mhT7ca8cUpRKBJuxZX7ejr5V9PMSQgujJ1ztHlr3g1WRjMAk7k";
        public static string oAuthVersion = "1.0";        
    }
    
    public class TwitterSettings
    {
        public static string RequestTokenUri = "https://api.twitter.com/oauth/request_token";
        public static string AuthorizeUri = "https://api.twitter.com/oauth/authorize";
        public static string AccessTokenUri = "https://api.twitter.com/oauth/access_token";
        public static string CallbackUri = "http://www.silvertouch.mobi/twitter/callback";//"http://www.bing.com";     
        public static string consumerKey ="dXhNZIycskB0tRIho0vLIbtPl";
        public static string consumerKeySecret ="glAlD97rDuOTaM05RmjD9exUMaHIyitjl7wX12pm3Cr977fTIt";
        public static string oAuthVersion = "1.0";
    }  
}
