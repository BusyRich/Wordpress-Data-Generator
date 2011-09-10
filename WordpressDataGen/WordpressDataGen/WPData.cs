using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Specialized;

namespace WordpressDataGen
{
    class WPData
    {
        private XDocument data = new XDocument();

        //These are xml namespaces used by wordpress
        private XNamespace wp= "http://wordpress.org/export/1.0/";
        private XNamespace wfw = "http://wellformedweb.org/CommentAPI/";
        private XNamespace dc = "http://purl.org/dc/elements/1.1/";
        private XNamespace content = "http://purl.org/rss/1.0/modules/content/";
        private XNamespace excerpt = "http://wordpress.org/export/1.1/excerpt/";

        private OrderedDictionary categories = new OrderedDictionary();
        private OrderedDictionary tags = new OrderedDictionary();

        private User[] users = new User[] {
            new User("'Cleric' Woods"),
            new User("'Emperor' King"),
            new User("'Laborer' Jenkins"),
            new User("'Prince' Hamilton"),
            new User("Billy 'The God' Wright"),
            new User("Carlos 'The Librarian' Ross"),
            new User("Elegant Elliott"),
            new User("Fallen Baker"),
            new User("Gerald 'Sacred Captain' Watkins"),
            new User("Hercules 'Wise Laborer' Greene"),
            new User("Infernal Y. X. Walker"),
            new User("Jeffrey 'The Mobster' Phillips"),
            new User("Johnny 'Foul' Williams"),
            new User("Juan the Lie"),
            new User("Justin 'Great God' Patterson"),
            new User("K. T. 'The CEO' Ellis"),
            new User("Larry the Flash"),
            new User("Mark 'Inkeeper Count' Mason"),
            new User("Peter 'Face' Torres"),
            new User("Sean 'Demonic Dancer' Martinez"),
            new User("Suave Thomas"),
            new User("The Anaconda Doctor"),
            new User("Unholy Terry Butler"),
            new User("ictor 'Mystery Warfare' Ramirez"),
            new User("X. J. 'The Wolf' Reed")
        };

        private Random rand = new Random();
        String dtFormat = "yyyy-MM-dd HH:mm:ss";

        public WPData()
        {
            data.Declaration = new XDeclaration("1.0", "utf-8", "yes");

            data.Add(
                new XElement("rss",
                    new XAttribute("version", "2.0"),
                    new XAttribute(XNamespace.Xmlns + "wp", wp),
                    new XAttribute(XNamespace.Xmlns + "wfw", wfw),
                    new XAttribute(XNamespace.Xmlns + "dc", dc),
                    new XAttribute(XNamespace.Xmlns + "content", content),
                    new XElement("channel",
                        new XElement(wp + "wxr_version", "1.1"),
                        new XComment("Generate some basic wordpress metadata"),
                        new XElement("title", "WP Dummy Test Data"),
                        new XElement("description", "Data to fill a wordpress blog for testing."),
                        new XElement("language", "en"),
                        new XElement("pubdate", DateTime.Now.ToString()),
                        new XElement("generator", "Richard's Super Awesome Wordpress Data Generator"),
                        new XComment("Generate some categories to use")
                    )
                )
            );

            GenerateCategories("Awesome Category");
            GenerateTags("Mega Tag");

            for (int p = 0; p < 100; p++)
            {
                //Pick a date within the lash year
                DateTime pubdate = DateTime.Now.AddMinutes(rand.NextDouble() * -525600);
                data.Element("rss").Element("channel").Add(
                    BuildItem(
                        "Test Post #" + p.ToString() + "[" + Guid.NewGuid().ToString().ToUpper()  + "]", 
                        pubdate
                ));
            }
        }

        public String GetDocumentAsString()
        {
            StringBuilder xs = new StringBuilder();
            xs.AppendLine(data.Declaration.ToString());
            xs.Append(data.ToString());

            return xs.ToString();
        }

        public void SaveToFile(String path)
        {
            String fileName = "\\wpdata" + rand.Next(1, 10000).ToString() + ".xml";
            data.Save(path + fileName);
        }

        private XElement GetCategory(String name, String parent)
        {
            String nicename = Regex.Replace(name, "[^0-9A-Za-z]", "");

            XElement cat = new XElement(wp + "category",
                new XElement(wp + "category_nicename", nicename),
                new XElement(wp + "category_parent", parent),
                new XElement(wp + "cat_name", new XCData(name))
            );

            categories.Add(name, nicename);

            return cat;
        }

        private XElement GetTag(String name)
        {
            String slug = Regex.Replace(name, "[^0-9A-Za-z]", "");

            XElement tag = new XElement(wp + "tag",
                new XElement(wp + "tag_slug", slug),
                new XElement(wp + "tag_name", new XCData(name))
            );

            tags.Add(name, slug);

            return tag;
        }

        private String BuildContent(int size)
        {
            StringBuilder content = new StringBuilder();

            for (int p = 0; p < size; p++)
            {
                content.AppendLine(
                    GenerateParagraph() +
                    Environment.NewLine);
            }

            return content.ToString().TrimEnd();
        }

        private XElement BuildItem(String title, DateTime date)
        {
            String postContent = BuildContent(rand.Next(5, 20));
            String postExcerpt = postContent.Substring(0, 150);

            XElement item = new XElement("item",
                new XElement("title", title),
                new XElement("pubdate", date.ToString(dtFormat)),
                new XElement(wp + "post_date", date.ToString(dtFormat)),
                new XElement(dc + "creator", "admin"),
                new XElement(wp + "post_type", "post"),
                new XElement(wp + "status", "publish"),
                new XElement(excerpt + "encoded", new XCData(postExcerpt)),
                new XElement(content + "encoded", new XCData(postContent))
            );

            //Add some random categories
            RandomizeDictionary(ref categories);
            object[] catNames = new object[categories.Keys.Count];
            categories.Keys.CopyTo(catNames, 0);
            for (int c = 0; c < rand.Next(2, categories.Count); c++)
            {
                item.Add(new XElement("category",
                    new XAttribute("domain", "category"),
                    new XAttribute("nicename", categories[c]),
                    new XCData(catNames[c].ToString())
                ));
            }

            //Add some random tags
            RandomizeDictionary(ref tags);
            object[] tagNames = new object[tags.Count];
            tags.Keys.CopyTo(tagNames, 0);
            for(int t = 0; t < rand.Next(2, tags.Count); t++)
            {
                item.Add(new XElement("category",
                    new XAttribute("domain", "tag"),
                    new XAttribute("nicename", tags[t]),
                    new XCData(tagNames[t].ToString())
                ));
            }

            BuildCommentTree(ref item, date);

            return item;
        }

        private XElement GenerateComment(DateTime date, int id, int parent)
        {
            User commenter = users[rand.Next(0, users.Length - 1)];
            String commentContent = BuildContent(rand.Next(1, 5));

            XElement comment = new XElement(wp + "comment",
                new XElement(wp + "comment_id", id),
                new XElement(wp + "comment_author", new XCData(commenter.username)),
                new XElement(wp + "comment_author_email", commenter.email),
                new XElement(wp + "comment_author_url", commenter.url),
                new XElement(wp + "comment_author_IP", commenter.ip),
                new XElement(wp + "comment_date", date.ToString(dtFormat)),
                new XElement(wp + "comment_content", commentContent),
                new XElement(wp + "comment_approved", 1),
                new XElement(wp + "comment_parent", parent)
            );

            return comment;
        }

        /* This adds three levels of categories to the document */
        private void GenerateCategories(String baseName)
        {
            XElement newCat;
            for (int p = 1; p < rand.Next(3, 10); p++)
            {
                newCat = GetCategory(baseName + " 0" + p.ToString(), "");
                data.Element("rss").Element("channel").Add(newCat);

                for (int c = 1; c < rand.Next(2, 5); c++)
                {
                    newCat = GetCategory(
                        baseName + " 0" + p.ToString() + ".0" + c.ToString(),
                        baseName + " 0" + p.ToString()
                    );
                    data.Element("rss").Element("channel").Add(newCat);

                    for (int gc = 1; gc < rand.Next(1, 3); gc++)
                    {
                        newCat = GetCategory(
                            baseName + " 0" + p.ToString() + ".0" + c.ToString() + ".0" + gc.ToString(),
                            baseName + " 0" + p.ToString() + ".0" + c.ToString()
                        );
                        data.Element("rss").Element("channel").Add(newCat);
                    }
                } 
            }
        }

        private void GenerateTags(String baseName)
        {
            for (int t = 0; t < rand.Next(5, 20); t++)
            {
                data.Element("rss").Element("channel").Add(
                    GetTag(baseName + " 0" + t.ToString()) 
                );
            }
        }

        public void BuildCommentTree(ref XElement item, DateTime postDate)
        {
            int rootID = rand.Next(1000, 5000);
            int childChance;

            for (int cc = 0; cc < rand.Next(2, 30); cc++)
            {   
                rootID++;
                postDate = postDate.AddHours(rand.Next(1, 24));
                childChance = rand.Next(0, 100);

                if (cc > 0 && childChance > 50)
                    item.Add(GenerateComment(postDate, rootID + cc, rootID--));
                else
                    item.Add(GenerateComment(postDate, rootID + cc, 0));      
            }
        }

        #region Utility Methods

        private String GenerateParagraph()
        {
            StringBuilder p = new StringBuilder();

            StringBuilder word = new StringBuilder();
            for (int w = 0; w < rand.Next(50, 400); w++)
            {
                word.Clear();
                for (int l = 0; l < rand.Next(2, 12); l++)
                {
                    word.Append((char)rand.Next(97, 122));
                }
                p.Append(word.ToString() + " ");
            }

            return "<p>" + p.ToString().TrimEnd() + "</p>";
        }

        private void RandomizeDictionary(ref OrderedDictionary dict)
        {
            OrderedDictionary newDict = new OrderedDictionary();

            int i = 0;
            while (dict.Count > 0)
            {
                object[] keys = new object[dict.Count];
                dict.Keys.CopyTo(keys, 0);
                object[] values = new object[dict.Count];
                dict.Values.CopyTo(values, 0);

                i = rand.Next(0, dict.Count - 1);
                newDict.Add(keys[i], values[i]);
                dict.RemoveAt(i);
            }

            dict = newDict;
        }

        #endregion
    }
}
