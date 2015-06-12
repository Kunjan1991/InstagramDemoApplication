using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OnesocialBackgroundAgent
{
    public class Home
    {
        public List<Datum> data { get; set; }        
    }
    public class Datum
    {
        public string id { get; set; }                
    }
    public class RootObjectTwt
    {
        public string created_at { get; set; }
        public object id { get; set; }
        public string id_str { get; set; }              
    }
    public class RootObjectLinkedIn
    {
        public int _count { get; set; }
    }
    public class RootObjectInstagram
    {       
        public List<Datum> data { get; set; }
    }
    public class RootObject
    {       
        public Response response { get; set; }
    }
    public class Response
    {
        public List<Post> posts { get; set; }
        public List<Item> items { get; set; }
    }
    public class Item
    {      
        public int source_id { get; set; }       
        public int post_id { get; set; }
    }
    public class Post
    {
        public string blog_name { get; set; }
        public object id { get; set; }
    }
    public class RootObjectNews
    {
        public Response response { get; set; }
    }

   
    public class IsoStorageData
    {          
        public string FacebookBaseValueCount { get; set; }         
        public string TwitterBaseValueCount { get; set; }        
        public string LinkedInBaseValueCount { get; set; }           
        public string InstagramBaseValueCount { get; set; }          
        public string TumblrBaseValueCount { get; set; }          
        public string VkontakteBaseValueCount { get; set; }
    }

}
