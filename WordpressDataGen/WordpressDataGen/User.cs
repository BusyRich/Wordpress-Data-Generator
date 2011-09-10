using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WordpressDataGen
{
    class User
    {
        public String username;
        public String email;
        public String url;
        public String ip;

        public User(String name)
        {
            username = name;

            String xName = Regex.Replace(name, "[^0-9A-Za-z]", "");
            email = xName + "@" + xName + ".com";
            url = "http://www." + xName + ".com";

            Random rand = new Random();
            for (int n = 0; n < 4; n++)
            {
                ip += rand.Next(1, 255).ToString() + ".";
            }
            ip = ip.TrimEnd(new char[] { '.' });
        }
    }
}
