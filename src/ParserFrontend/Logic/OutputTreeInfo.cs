using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ParserFrontend.Logic
{
    public class OutputTreeInfo
    {
        public class LineInfo
        {
            public int Ident;
            public string Text;
            public int? Page;
            public int? Id;
        }

        public class Node
        {
            public static Node Empty = new Node();

            public readonly LineInfo Line;
            public List<Node> Children;
            public int Ident => Line.Ident;
            public string Title => Line.Text;

            public static Node CreateRoot()
            {
                return new Node();
            }

            private Node()
            {
                this.Line = new LineInfo { Ident = -1 };
            }

            public Node(LineInfo line)
            {
                this.Line = line;
            }

            public void Add(Node node)
            {
                if (Children == null)
                    Children = new List<Node>();

                Children.Add(node);
            }
        }

        static Regex PATTERN_SIMPLE = new Regex(@"(\s*)(.*)");
        static Regex PATTERN_PAGE_ID = new Regex(@"(\s*)(.*)(\s\(Page (\d+), ID=(\d+)\))");

        public Node Process(string text)
        {
            var lines = GenerateLines(text).ToArray();

            Stack<Node> nodeStack = new Stack<Node>();
            Node rootNode = Node.CreateRoot();
            Node currentNode = rootNode;

            foreach (var line in lines)
            {
                // Parent
                while (line.Ident <= currentNode.Ident)
                {
                    currentNode = nodeStack.Pop();
                }

                // Add as child
                var node = new Node(line);
                currentNode.Add(node);

                // Update the child
                nodeStack.Push(currentNode);
                currentNode = node;
            }

            return rootNode;
        }


        IEnumerable<LineInfo> GenerateLines(string text)
        {
            Func<char,bool> NOT_SPACE = (ch => ch != ' ');

            var lines = text.Replace("\r", "").Split('\n');

            foreach (string line in lines)
            {
                if (line.Trim() == "")
                    continue;

                var matches = PATTERN_PAGE_ID.Match(line);

                LineInfo lineInfo;

                if (matches.Success)
                {
                    lineInfo = new LineInfo()
                    {
                        Ident = matches.Groups[1].Length,
                        Text = matches.Groups[2].Value,
                        Page = Int32.Parse(matches.Groups[4].Value),
                        Id = Int32.Parse(matches.Groups[5].Value)
                    };
                }
                else
                {
                    var simpleMatch = PATTERN_SIMPLE.Match(line);

                    lineInfo = new LineInfo
                    {
                        Ident = simpleMatch.Groups[1].Length,
                        Text = simpleMatch.Groups[2].Value,
                        Page = -1,
                        Id = -1
                    };
                }

                yield return lineInfo;
            }
        }
    }
}
