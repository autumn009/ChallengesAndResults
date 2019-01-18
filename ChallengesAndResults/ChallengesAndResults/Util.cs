using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ChallengesAndResults
{
    public class Util
    {
        public static List<string> Names;
        public static string ManifestName;

        public static void SetResource(string name, string cname)
        {
            ManifestName = cname;
            Names = new List<string>();
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream("ChallengesAndResults." + name))
            {
                using (var reader = new StreamReader(stream))
                {
                    for (; ; )
                    {
                        var s = reader.ReadLine();
                        if (s == null) return;
                        if (s.StartsWith("#")) continue;
                        Names.Add(s);
                    }
                }
            }
        }
    }
}
