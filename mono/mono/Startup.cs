using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(mono.Startup))]
namespace mono
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
