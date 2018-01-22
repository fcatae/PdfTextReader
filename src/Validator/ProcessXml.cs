using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Validator
{
    class ProcessXml : IRunner
    {
        public string FilePattern => "*.xml";

        public void Run(File file, string outputname)
        {
            string inputFolder = file.Folder;
            string basename = file.Filename;

            Console.WriteLine($"Output xml file = {basename}");

            Article Read()
            {

                var xmlFile = inputFolder + "\\" + basename + ".xml";

                using (StreamReader sr = new StreamReader(xmlFile))
                {
                    string article = sr.ReadToEnd();


                    var title = FindTitleArticles(article);
                    var caput = FindCaputArticles(article);
                    var body = FindBodyArticles(article);

                    if (title == null)
                        throw new ApplicationException();
                    else
                        Console.WriteLine("Titulo OK  ---------" + title );
                    

                    if (caput == null)
                        throw new ApplicationException();
                    else
                        Console.WriteLine("caput OK  ---------" + caput);
                    if (body == null)
                        throw new ApplicationException();
                    else
                        Console.WriteLine("body OK  ---------" + body);



                    return new Article()
                    {
                        Title = title,
                        Caput = caput,
                        Body = body
                    };
                }

            }
        }

            string FindTitleArticles(string article)
            {
                string title = "";
                string cleanArticle = article;

                cleanArticle = cleanArticle.Replace("\n", "");
                cleanArticle = cleanArticle.Replace("\r", "");

                var pattern = @"<Titulo>(.*)</Titulo>";
                Regex rgx = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

                MatchCollection matches = rgx.Matches(cleanArticle);


                if (matches.Count > 0)
                {
                    Match match = matches[0];


                    //Tratamento do titulo

                    title = match.Value;
                    title = title.Replace("<Titulo>", "");
                    title = title.Replace("</Titulo>", "");

                    return title;

                }
                return null;

            }

            string FindCaputArticles(string article)
            {
                var pattern = @"<caput>(.*)</caput>";
                Regex rgx = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

                MatchCollection matches = rgx.Matches(article);

                string caput = "";


                if (matches.Count > 0)
                {
                    Match match = matches[0];


                    //Tratamento do titulo

                    caput = match.Value;
                    caput = caput.Replace("<caput>", "");
                    caput = caput.Replace("</caput>", "");

                }

                return caput;

            }

            string FindBodyArticles(string article)
            {
                var pattern = @"<corpo>(.*)</corpo>";
                Regex rgx = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

                MatchCollection matches = rgx.Matches(article);

                string body = "";

                if (matches.Count > 0)
                {
                    Match match = matches[0];


                    //Tratamento do titulo

                    body = match.Value;
                    body = body.Replace("<Corpo>", "");
                    body = body.Replace("</Corpo>", "");

                    return body;

                }

                return body;

            }











        }

        class Article
        {
            public string Title { get; set; }
            public string Caput { get; set; }
            public string Body { get; set; }
        }

        

    



    
}
