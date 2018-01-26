using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Validator
{
    class ProcessXml : IRunner
    {
        public string _inputFolder { get; set; }
        public string _basename { get; set; }

        public static string _xmlFile { get; set; }
        public static double errorcounter { get; set; } = 0.00;
        public static double successcounter { get; set; } = 0.00;



        public string FilePattern => "*.xml";

        public void Run(File file, string outputname)
        {
            _inputFolder = file.Folder;
            _basename = file.Filename;

            _xmlFile = _inputFolder + "\\" + _basename + ".xml";

            var xmlFile = _inputFolder + "\\" + _basename + ".xml";

            var article = Read();

            if (article.Body == null || article.Title == null || article.Caput == null)
            {

                errorcounter++;
                ErrorLogFile($"Article Title or Body or Caput are null ------- File: +  {xmlFile}");
            }
            else
            {
                Console.WriteLine($"XML OK  --------- {_basename}");
                successcounter++;
            }

            ValidateAnexoArticles(article);

           

            Console.WriteLine($"Output xml file = {_basename}");

        }

        Article Read()
        {

            var xmlFile = _inputFolder + "\\" + _basename + ".xml";

            using (StreamReader sr = PdfTextReader.Base.VirtualFS.OpenStreamReader(xmlFile))
            {
                string article = sr.ReadToEnd();


                var title = FindTitleArticles(article);
                var caput = FindCaputArticles(article);
                var body = FindBodyArticles(article);


                FindSignatureArticles(article);

                return new Article()
                {
                    Title = title,
                    Caput = caput,
                    Body = body
                };
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

            string body = null;

            if (matches.Count > 0)
            {
                Match match = matches[0];


                //Tratamento do titulo

                body = match.Value;
                body = body.Replace("<Corpo>", "");
                body = body.Replace("</Corpo>", "");

                if(body.Length < 55)
                {
                    errorcounter++;
                    ErrorLogFile($"Body content are less than 55 words -------- {_xmlFile}");
                }

                return body;

            }

            return body;

        }

        void FindSignatureArticles(string article)
        {
            var pattern = @"<Assinatura>.*?(?=</Assinatura>)";
            Regex rgx = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

            MatchCollection matches = rgx.Matches(article);


            if (matches.Count > 0)
            {
             foreach (Match match in matches)
                {
                    var signature = match.Value;
                    signature = signature.Replace("<Autor>", "");
                    signature = signature.Replace("</Autor>", "");

                    var sigA = signature.Substring(1,2);
                    var sigB = signature.Substring(1,2);
                    sigB = sigB.ToLower();

                    if (sigA == sigB)
                    {
                        errorcounter++;
                        ErrorLogFile($"Signature is wrong -------- {_xmlFile}");
                    }
                 }

            }

        }











        static void ValidateAnexoArticles(Article article)
        {

            if (article.Title.ToLower().Contains("anexo") == true)
            {
                string lastLine = article.Body;
                if(article.Body.Length > 70)
                {
                    lastLine = lastLine.Substring((article.Body.Length - 70),70);
                }             
                
                var xmlFileCheck = @"C:\DOUtests\output\2012_01_02_p_anvisa\2012_01_02_p_anvisa-artigo39.xml";

                   if (xmlFileCheck == _xmlFile )
                    {
                        Console.WriteLine("Error Checking");
                    }

                var pattern = @"(Portaria nº(-)?|Portaria no(-)?|Lei nº|Lei no|Decreto no(s)?|Decreto nº|Resolução - RE Nº) ([0-9]+(\.[0-9]+)?(\-[0-9]+)?)";
                    Regex rgx = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

                    var matchResult = rgx.Match(lastLine);

                    if (matchResult.Success == true)
                    {
                        errorcounter++;
                        ErrorLogFile($"In the ANEXO article was found another Title Article at the end of the BODY structure ------- File: +  {_xmlFile}");
            
                    }

            }



       }

        static void ErrorLogFile(string log)
        {
            using (StreamWriter sw = new StreamWriter(@"C:\DOUtests\errorlog\log.txt", true))
            {
                sw.WriteLine(log);
            }


        }







    }

        class Article
        {
            public string Title { get; set; }
            public string Caput { get; set; }
            public string Body { get; set; }
        }





    }


