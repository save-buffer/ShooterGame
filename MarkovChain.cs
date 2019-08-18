using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ShooterGame
{
    class MarkovChain
    {
        const string START = "START";
        const string END = "_";
        Dictionary<string, string> links;
        Random rand;
        string Filename;

        public MarkovChain(string File)
        {
            rand = new Random();
            links = new Dictionary<string, string>();
            Filename = File;
        }

        public void GenerateChain()
        {
            links.Clear();

            string[] lines = File.ReadAllLines("Assets\\" + Filename);
            foreach (string e in lines)
            {
                string element = e.ToLower();
                for (int i = 0; i < element.Length; i++)
                {
                    if (i == 0)
                    {
                        AddElement(element.Substring(0, 1), element.Substring(1, 1));
                        AddElement(START, element.Substring(0, 1));
                    }
                    else if (i == element.Length - 1)
                        AddElement(element.Substring(i, 1), END);
                    else
                        AddElement(element.Substring(i, 1), element.Substring(i + 1, 1));
                }
            }

            foreach(KeyValuePair<string, string> item in links)
            {
                Console.WriteLine(item.Key + " => " + item.Value);
            }
        }

        public string GenerateElement()
        {
            string result = links[START].Substring(rand.Next(links[START].Length), 1);
            while (result.Length < 10 && result.Substring(result.Length - 1, 1) != "_")
            {
                int tries = 0;
                string key = result.Substring(result.Length - 1, 1);
                while (key == "_" && result.Length < 4 && tries++ < 10)
                    key = result.Substring(result.Length - 1, 1);
                if (tries >= 15)
                    break;

                string w = links[key].Substring(rand.Next(links[key].Length), 1);

                while (w == "_" && result.Length < 4 && tries++ < 10)
                    w = links[key].Substring(rand.Next(links[key].Length), 1);
                if (tries >= 15)
                    break;

                result += w;
            }
            return result.Substring(0, result.Length - 1);
        }

        public void AddElement(string key, string value)
        {
            if (links.ContainsKey(key))
                links[key] += value;
            else
                links.Add(key, value);
        }
    }
}
