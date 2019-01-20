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
        public static int TryRate = 0;
        public static int CorrectRate = 0;
        public static List<bool> TryChecks;
        public static List<bool> CorrectChecks;

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
                        if (s == null) break;
                        if (s.StartsWith("#")) continue;
                        Names.Add(s);
                    }
                }
            }
            TryChecks = Enumerable.Repeat(true, Names.Count()).ToList();
            CorrectChecks = Enumerable.Repeat(false, Names.Count()).ToList();
        }
        public static int GetTryRate() => TryChecks.Count(c => c) * 100 / TryChecks.Count();
        public static int GetCorrectRate() => CorrectChecks.Count(c => c) * 100 / CorrectChecks.Count();
    }
}
