using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebBDS.Startup))]
namespace WebBDS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
