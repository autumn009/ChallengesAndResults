using Blazor.Extensions.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor.Components;
using Newtonsoft.Json;
using System.Text;
using Microsoft.JSInterop;

namespace ChallengesAndResults
{
    public class Util
    {
        public static List<string> Names;
        public static string ManifestName;
        public static dynamic PageRefer;
        public static int TryRate = 0;
        public static int CorrectRate = 0;
        public static List<bool> TryChecks;
        public static List<bool> CorrectChecks;
        public static LocalStorage localStorage { get; set; }
        public static Microsoft.AspNetCore.Blazor.Services.IUriHelper uriHelper { get; set; }

        public static async Task SetResource(string name, string cname)
        {
            ManifestName = cname;
            Names = new List<string>();
            Assembly assembly = Assembly.GetExecutingAssembly();
            //Console.WriteLine(assembly.FullName);
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
            await Load();
        }
        public static int GetTryRate() => TryChecks.Count(c => c) * 100 / TryChecks.Count();
        public static int GetCorrectRate()
        {
            int r = TryChecks.Count(c => c);
            if (r == 0) return 0;
            return CorrectChecks.Count(c => c) * 100 / r;
        }
        public static void MainLayoutInitializeEntry(LocalStorage localStorage0, Microsoft.AspNetCore.Blazor.Services.IUriHelper uriHelper0, object pageRefer0)
        {
            localStorage = localStorage0;
            uriHelper = uriHelper0;
            //PageRefer = pageRefer0;
        }

        private static async Task LoadBoolArray(List<bool> target, string name)
        {
            var r = await localStorage.GetItem<bool[]>(name + "_" + ManifestName);
            if (r != null)
            {
                for (int i = 0; i < Math.Min(r.Length, Names.Count); i++)
                {
                    target[i] = r[i];
                }
            }
        }
        private static async Task SaveBoolArray(List<bool> target, string name)
        {
            if (target == null) return;
            await localStorage.SetItem<bool[]>(name + "_" + ManifestName, target.ToArray());
        }
        private static async Task Load()
        {
            //Console.WriteLine("Load Callded");
            clear();
            await LoadBoolArray(TryChecks, "try");
            await LoadBoolArray(CorrectChecks, "correct");
        }

        public static void clear()
        {
            TryChecks = Enumerable.Repeat(false, Names.Count()).ToList();
            CorrectChecks = Enumerable.Repeat(false, Names.Count()).ToList();
        }

        public static async Task Save()
        {
            //Console.WriteLine("Save Callded");
            await SaveBoolArray(TryChecks, "try");
            await SaveBoolArray(CorrectChecks, "correct");
        }
        class SerializeContainer
        {
            public bool[] tryChecks;
            public bool[] correctChecks;
        }
        // this is workaround for Blazor 0.70
        // https://github.com/mono/mono/issues/11848
        public static void Workaround()
        {
            typeof(System.Collections.Specialized.INotifyCollectionChanged).GetHashCode();
            typeof(System.ComponentModel.INotifyPropertyChanged).GetHashCode();
            typeof(System.ComponentModel.INotifyPropertyChanging).GetHashCode();
        }
        public static string Export()
        {
            Workaround();
            var container = new SerializeContainer();
            container.tryChecks = TryChecks.ToArray();
            container.correctChecks = CorrectChecks.ToArray();
            var json = JsonConvert.SerializeObject(container);
            var bytear = Encoding.UTF8.GetBytes(json);
            return "data:application/octet-stream;base64," + Convert.ToBase64String(bytear);
        }
        [JSInvokable]
        public static void Upload(string base64)
        {
            Console.WriteLine("Upload called");
            Workaround();
            clear();
            byte[] bytes = Convert.FromBase64String(base64);
            var s = Encoding.UTF8.GetString(bytes);
            Console.WriteLine(s);
            var deserialized = JsonConvert.DeserializeObject<SerializeContainer>(s);
            Console.WriteLine(deserialized.tryChecks.Count());
            for (int i = 0; i < Math.Min(Names.Count(), deserialized.tryChecks.Count()); i++)
            {
                Console.WriteLine(deserialized.tryChecks[i]);
                TryChecks[i] = deserialized.tryChecks[i];
            }
            for (int i = 0; i < Math.Min(Names.Count(), deserialized.correctChecks.Count()); i++)
            {
                Console.WriteLine(deserialized.correctChecks[i]);
                CorrectChecks[i] = deserialized.correctChecks[i];
            }
            PageRefer.StateHasChangedProxy();
            _ = Util.Save();
        }
    }
}
