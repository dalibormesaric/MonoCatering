using Microsoft.Owin;
using Owin;
using System.Globalization;

[assembly: OwinStartupAttribute(typeof(Mono.Startup))]
namespace Mono
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
