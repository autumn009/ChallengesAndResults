using Blazor.Extensions.Storage;
using Microsoft.AspNetCore.Blazor.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ChallengesAndResults
{
    public class Startup
    {
        // this is workaround for Blazor 0.70
        // https://github.com/mono/mono/issues/11848
        static Startup()
        {
            typeof(System.ComponentModel.INotifyPropertyChanging).GetHashCode();
            typeof(System.ComponentModel.INotifyPropertyChanged).GetHashCode();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add Blazor.Extensions.Storage
            // Both SessionStorage and LocalStorage are registered
            var r = services.AddStorage();
        }

        public void Configure(IBlazorApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
