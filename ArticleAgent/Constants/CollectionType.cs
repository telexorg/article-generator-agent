using ArticleAgent.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.VisualBasic;
using System.Reflection.Metadata;

namespace ArticleAgent.Constants
{
    public class CollectionType
    {
        public const string User = "Users";
        public const string Convesation = "Conversations";
        public const string Company = "Organizations";
        public const string Message = "Messages";
        public const string Blog = "Blogs";
        public const string ApiKey = "ApiKeys";
        public const string BlogTask = "BlogTasks";

        public static string ResolveTagName<T>()
        {
            var type = typeof(T);

            return 
                   type == typeof(Message) ? Message :
                   type.Name; // Fallback: use raw type name
        }

    }
}
