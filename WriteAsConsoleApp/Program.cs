using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WriteAs.Client;
using WriteAs.Client.Models;

namespace WriteAsConsoleApp
{
    class Program
    {
        private static readonly string WriteAsApiUri = @"https://write.as/";
        static async Task Main(string[] args)
        {
            try
            {
                await ProcessCommand(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", ex.Message);
            }
        }

        private static async Task ProcessCommand(string[] args)
        {
            if (string.Compare(args[0], "-h", ignoreCase: true) == 0 || string.Compare(args[0], "--help", ignoreCase: true) == 0)
            {
                DisplayHelp();
            } 
            else if (string.Compare(args[0], "-V", ignoreCase: true) == 0 || string.Compare(args[0], "--version", ignoreCase: true) == 0)
            {
                DisplayVersion();
            }
            else
            {
                string operation = args[0];

                if (string.Compare(operation, "GetAllPosts", ignoreCase: true) == 0)
                {
                    string alias = args[1];
                    SortOrder sortOrder = GetSorOrderParameter(args, argsIndex: 2);
                    await GetAllPosts(alias, sortOrder);
                }
                else if (string.Compare(operation, "GetPostsByPageNumber", ignoreCase: true) == 0)
                {
                    string alias = args[1];
                    int pageNumber = GetPageNumberParameter(args, argsIndex: 2);
                    SortOrder sortOrder = GetSorOrderParameter(args, argsIndex: 3);
                    await GetPostsByPageNumber(alias, pageNumber, sortOrder);
                }
                else if (string.Compare(operation, "GetPostBySlug", ignoreCase: true) == 0)
                {
                    string alias = args[1];
                    string slug = args[2];
                    await GetPostBySlug(alias, slug);
                }
                else if (string.Compare(operation, "GetPostById", ignoreCase: true) == 0)
                {
                    string postId = args[1];
                    await GetPostById(postId);
                }
                else if (string.Compare(operation, "Search", ignoreCase: true) == 0)
                {
                    string alias = args[1];
                    string searchKey = GetSearchKey(args, 2);
                    await Search(alias, searchKey);
                }
                else
                {
                    Console.WriteLine("Cannot process command...");
                    DisplayHelp();
                }
            }
        }

        private static void DisplayHelp()
        {
            StringBuilder helpText = new StringBuilder();
            helpText.AppendLine("USAGE: WriteAsConsoleApp [options] operation [arguments]");
            helpText.AppendLine();
            helpText.AppendLine("OPTIONS:");
            helpText.AppendLine("   -h|--help           Show command line help.");
            helpText.AppendLine("   -v|--version        Display console app version.");
            helpText.AppendLine();
            helpText.AppendLine("OPERATIONS:");
            helpText.AppendLine("    GetAllPosts <alias> [sortorder]");
            helpText.AppendLine("    GetPostsByPageNumber <alias> <pagenumber> [sortorder]");
            helpText.AppendLine("    GetPostBySlug <alias> <slug>");
            helpText.AppendLine("    GetPostById <id>");
            helpText.AppendLine("    Search <alias> <searchKey>");
            helpText.AppendLine();
            helpText.AppendLine("SORTORDER-options:");
            helpText.AppendLine("    ascending");
            helpText.AppendLine("    descending");
            Console.WriteLine(helpText.ToString());
        }

        private static void DisplayVersion()
        {
            Console.WriteLine("WriteAsConsoleApp Version: " + Assembly.GetExecutingAssembly().GetName().Version);
        }

        private static int GetPageNumberParameter(string[] args, int argsIndex)
        {
            try
            {
                return Convert.ToInt32(args[argsIndex]);
            }
            catch (Exception)
            {
                Console.WriteLine("Cannot parse PageNumber parameter value, so returning default value of 1");
                return 1;
            }
        }

        private static SortOrder GetSorOrderParameter(string[] args, int argsIndex)
        {
            try
            {
                if (string.Compare("ascending", args[argsIndex], ignoreCase: true) == 0)
                {
                    return SortOrder.Ascending;
                }
                else
                {
                    return SortOrder.Descending;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Cannot parse SortOrder value, so returning default value, SortOrder.Descending");
                return SortOrder.Descending;
            }
        }

        private static string GetSearchKey(string[] args, int argsIndex)
        {
            string searchKey = string.Empty;
            for (int i = argsIndex; i < args.Length; i++)
            {
                searchKey += args[i];
                if ((i + 1) < args.Length)
                {
                    searchKey += " ";
                }
            }

            return searchKey;
        }

        private static async Task GetAllPosts(string alias, SortOrder sortOrder = SortOrder.Descending)
        {
            Console.WriteLine("OPERATION: GetAllPosts");
            Console.WriteLine("ALIAS: " + alias);
            Console.WriteLine("SORTORDER: " + sortOrder.ToString());
            using (var client = new WriteAsClient(WriteAsApiUri))
            {
                List<Post> allPosts = await client.GetAllPosts(alias, sortOrder);

                foreach (var post in allPosts)
                {
                    string title = string.IsNullOrEmpty(post.Title) ? "Untitled Post" : post.Title;
                    Console.WriteLine(string.Format("{0}: {1}", post.CreateDate.ToString("yyyy-MM-dd"), title));
                }
            }
        }
        private static async Task GetPostsByPageNumber(string alias, int pageNumber, SortOrder sortOrder = SortOrder.Descending)
        {
            Console.WriteLine("OPERATION: GetPostsByPageNumber");
            Console.WriteLine("ALIAS: " + alias);
            Console.WriteLine("PAGENUMBER: " + pageNumber);
            Console.WriteLine("SORTORDER: " + sortOrder.ToString());
            using (var client = new WriteAsClient(WriteAsApiUri))
            {
                List<Post> allPosts = await client.GetPostsByPageNumber(alias, pageNumber, sortOrder);

                foreach (var post in allPosts)
                {
                    string title = string.IsNullOrEmpty(post.Title) ? "Untitled Post" : post.Title;
                    Console.WriteLine(string.Format("{0}: {1}", post.CreateDate.ToString("yyyy-MM-dd"), title));
                }
            }
        }
        private static async Task GetPostBySlug(string alias, string slug)
        {
            Console.WriteLine("OPERATION: GetPostBySlug");
            Console.WriteLine("SLUG: " + slug);
            using (var client = new WriteAsClient(WriteAsApiUri))
            {
                Post post = await client.GetPostBySlug(alias, slug);
                if (post != null)
                {
                    string title = string.IsNullOrEmpty(post.Title) ? "Untitled Post" : post.Title;
                    Console.WriteLine(string.Format("{0}: {1}", post.CreateDate.ToString("yyyy-MM-dd"), title));
                }
                else
                {
                    Console.WriteLine(string.Format("Cannot find post with slug {0}", slug));
                }
            }
        }
        private static async Task GetPostById(string postId)
        {
            Console.WriteLine("OPERATION: GetPostById");
            Console.WriteLine("POSTID: " + postId);
            using (var client = new WriteAsClient(WriteAsApiUri))
            {
                Post post = await client.GetPostById(postId);
                if (post != null)
                {
                    string title = string.IsNullOrEmpty(post.Title) ? "Untitled Post" : post.Title;
                    Console.WriteLine(Environment.NewLine + "Result:" + Environment.NewLine 
                        + string.Format("{0}: {1}", post.CreateDate.ToString("yyyy-MM-dd"), title));
                }
                else
                {
                    Console.WriteLine(Environment.NewLine + "Result: " + string.Format("Cannot find post with ID {0}", postId));
                }
            }
        }
        private static async Task Search(string alias, string searchKey)
        {
            Console.WriteLine("OPERATION: Search");
            Console.WriteLine("SEARCHKEY: " + searchKey);
            using (var client = new WriteAsClient(WriteAsApiUri))
            {
                List<Post> searchResults = await client.Search(alias, searchKey);
                if (searchResults != null && searchResults.Count > 0)
                {
                    foreach (var post in searchResults)
                    {
                        string title = string.IsNullOrEmpty(post.Title) ? "Untitled Post" : post.Title;
                        Console.WriteLine(string.Format("{0}: {1}", post.CreateDate.ToString("yyyy-MM-dd"), title));
                    }
                }
                else
                {
                    Console.WriteLine(Environment.NewLine + "Result: " + string.Format("Could not find any post with searchKey {0}", searchKey));
                }
            }
        }
    }
}
