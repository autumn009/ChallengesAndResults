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
        class SerializeContainerItem
        {
            public string name;
            public bool tryCheck;
            public bool correctCheck;
        }
        class SerializeContainer
        {
            public SerializeContainerItem[] items;
        }

        private static async Task Load()
        {
            //Console.WriteLine("Load Callded");
            clear();
            var r = await localStorage.GetItem<SerializeContainer>("v2_" + ManifestName);
            parseContainer(r);
        }

        private static void parseContainer(SerializeContainer r)
        {
            for (int i = 0; i < Names.Count; i++)
            {
                var item = r.items.FirstOrDefault(c => c.name == Names[i]);
                if (item == null) continue;
                TryChecks[i] = item.tryCheck;
                CorrectChecks[i] = item.correctCheck;
            }
        }

        public static void clear()
        {
            TryChecks = Enumerable.Repeat(false, Names.Count()).ToList();
            CorrectChecks = Enumerable.Repeat(false, Names.Count()).ToList();
        }

        public static async Task Save()
        {
            //Console.WriteLine("Save Callded");
            SerializeContainer r = createContainer();
            await localStorage.SetItem<SerializeContainer>("v2_" + ManifestName, r);
        }

        private static SerializeContainer createContainer()
        {
            var r = new SerializeContainer();
            r.items = new SerializeContainerItem[Names.Count];
            for (int i = 0; i < Names.Count; i++)
            {
                r.items[i] = new SerializeContainerItem();
                r.items[i].name = Names[i];
                r.items[i].tryCheck = TryChecks[i];
                r.items[i].correctCheck = CorrectChecks[i];
            }

            return r;
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
            var container = createContainer();
            var json = JsonConvert.SerializeObject(container);
            var bytear = Encoding.UTF8.GetBytes(json);
            return "data:application/octet-stream;base64," + Convert.ToBase64String(bytear);
        }
        [JSInvokable]
        public static void Upload(string base64)
        {
            //Console.WriteLine("Upload called");
            Workaround();
            clear();
            byte[] bytes = Convert.FromBase64String(base64);
            var s = Encoding.UTF8.GetString(bytes);
            //Console.WriteLine(s);
            var deserialized = JsonConvert.DeserializeObject<SerializeContainer>(s);
            parseContainer(deserialized);
            PageRefer.StateHasChangedProxy();
            _ = Util.Save();
        }
    }
}
