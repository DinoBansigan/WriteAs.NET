using HeyRed.MarkdownSharp;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
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
                else if (string.Compare(operation, "GenerateArchivePageMarkdown", ignoreCase: true) == 0)
                {
                    string alias = args[1];
                    string fileName = args[2];
                    await GenerateArchivePageMarkdown(alias, fileName);
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
            helpText.AppendLine("    GenerateArchivePageMarkdown <alias> <filename> [sortorder]");
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
            Console.WriteLine("ALIAS: " + alias);
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
            Console.WriteLine("ALIAS: " + alias);
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

        private static async Task GenerateArchivePageMarkdown(string alias, string fileName, SortOrder sortOrder = SortOrder.Descending)
        {
            Console.WriteLine("OPERATION: GenerateArchivePageMarkdown");
            Console.WriteLine("ALIAS: " + alias);
            Console.WriteLine("FILENAME: " + fileName);
            Console.WriteLine("SORTORDER: " + sortOrder.ToString());
            using (var client = new WriteAsClient(WriteAsApiUri))
            {
                List<Post> allPosts = await client.GetAllPosts(alias, sortOrder);
                if (allPosts != null && allPosts.Count > 0)
                {
                    string outputFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "output");
                    if (!Directory.Exists(outputFolder))
                    {
                        Directory.CreateDirectory(outputFolder);
                    }
                    
                    StringBuilder stringBuilder = new StringBuilder();
                    string currentYear = DateTime.Now.Year.ToString();
                    stringBuilder.AppendLine("<div class=\"archivePage\">");
                    stringBuilder.AppendLine("<h1>" + currentYear + "</h1>");
                    stringBuilder.AppendLine("<hr class=\"archivePageHr\"/>");

                    foreach (var post in allPosts)
                    {
                        if (post.CreateDate.Year.ToString() != currentYear)
                        {
                            stringBuilder.AppendLine();
                            currentYear = post.CreateDate.Year.ToString();
                            stringBuilder.AppendLine("<h1>" + currentYear + "</h1>");
                            stringBuilder.AppendLine("<hr class=\"archivePageHr\"/>");
                        }

                        string createDate = post.CreateDate.ToString("yyyy-MM-dd");
                        string postTitle = GetPostTitle(post);
                        string postUrl = "https://journal.dinobansigan.com/" + post.Slug;
                        stringBuilder.AppendLine(string.Format("<div><a href=\"{0}\" target=\"_blank\"><span class=\"archivePageDateSpan\">{1}:</span> {2}</a></div>", postUrl, createDate, postTitle));
                    }
                    stringBuilder.AppendLine("</div>");

                    if (!fileName.ToLower().EndsWith(".txt"))
                    {
                        fileName += ".txt";
                    }
                    string outputFileName = Path.Combine(outputFolder, fileName);
                    Console.WriteLine("Saving markdown text to " + outputFileName);
                    File.WriteAllText(outputFileName, stringBuilder.ToString());
                    Console.WriteLine("Successfully saved Markdown text.");
                }
            }
        }

        private static string GetPostTitle(Post post)
        {
            string postTitle = string.Empty;
            if (string.IsNullOrEmpty(post.Title))
            {
                postTitle = "Untitled Post";
                string postContent = new Markdown().Transform(post.Body);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(postContent);

                using (StringWriter sw = new StringWriter())
                {
                    ConvertTo(doc.DocumentNode, sw);
                    sw.Flush();
                    string plainText = sw.ToString();
                    if (!string.IsNullOrEmpty(plainText))
                    {
                        plainText = plainText.Replace("\r", "");
                        plainText = plainText.Replace("\n", "");
                        int length = plainText.Length < 50 ? plainText.Length : 50;
                        postTitle = plainText.Substring(0, length) + "...";
                    }
                }
            }
            else
            {
                postTitle = post.Title;
            }

            postTitle = postTitle.Replace("#", "&#35;"); // this is to avoid triggering write.as' hashtag system
            return postTitle;
        }

        private static void ConvertTo(HtmlNode node, TextWriter outText)
        {
            string html;
            switch (node.NodeType)
            {
                case HtmlNodeType.Comment:
                    // don't output comments
                    break;

                case HtmlNodeType.Document:
                    ConvertContentTo(node, outText);
                    break;

                case HtmlNodeType.Text:
                    // script and style must not be output
                    string parentName = node.ParentNode.Name;
                    if ((parentName == "script") || (parentName == "style"))
                        break;

                    // get text
                    html = ((HtmlTextNode)node).Text;

                    // is it in fact a special closing node output as text?
                    if (HtmlNode.IsOverlappedClosingElement(html))
                        break;

                    // check the text is meaningful and not a bunch of whitespaces
                    if (html.Trim().Length > 0)
                    {
                        outText.Write(HtmlEntity.DeEntitize(html));
                    }
                    break;

                case HtmlNodeType.Element:
                    switch (node.Name)
                    {
                        case "p":
                            // treat paragraphs as crlf
                            outText.Write("\r\n");
                            break;
                        case "br":
                            outText.Write("\r\n");
                            break;
                    }

                    if (node.HasChildNodes)
                    {
                        ConvertContentTo(node, outText);
                    }
                    break;
            }
        }

        private static void ConvertContentTo(HtmlNode node, TextWriter outText)
        {
            foreach (HtmlNode subnode in node.ChildNodes)
            {
                ConvertTo(subnode, outText);
            }
        }
    }
}
