using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(JalpaBlog.Startup))]
namespace JalpaBlog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
