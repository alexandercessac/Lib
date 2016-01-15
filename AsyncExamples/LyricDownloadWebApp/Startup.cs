using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LyricDownloadWebApp.Startup))]
namespace LyricDownloadWebApp
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
