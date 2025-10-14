using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Meliasoft.Startup))]
namespace Meliasoft
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
