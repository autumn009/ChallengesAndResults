using Blazor.Extensions.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor.Components;

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
        public static LocalStorage localStorage { get; set; }
        public static Microsoft.AspNetCore.Blazor.Services.IUriHelper uriHelper { get; set; }

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
            TryChecks = Enumerable.Repeat(false, Names.Count()).ToList();
            CorrectChecks = Enumerable.Repeat(false, Names.Count()).ToList();
        }
        public static int GetTryRate() => TryChecks.Count(c => c) * 100 / TryChecks.Count();
        public static int GetCorrectRate()
        {
            int r = TryChecks.Count(c => c);
            if (r == 0) return 0;
            return CorrectChecks.Count(c => c) * 100 / r;
        }
        public static async Task MainLayoutInitializeEntry(LocalStorage localStorage0, Microsoft.AspNetCore.Blazor.Services.IUriHelper uriHelper0)
        {
            localStorage = localStorage0;
            uriHelper = uriHelper0;
            await Load();
        }

        private static async Task Load()
        {
            // TBW
            await Task.Delay(0);
            //throw new NotImplementedException();
        }
        private static async Task Save()
        {
            // TBW
            await Task.Delay(0);
            //throw new NotImplementedException();
        }
    }
}
