using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PicsOfUs.Startup))]
namespace PicsOfUs
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
