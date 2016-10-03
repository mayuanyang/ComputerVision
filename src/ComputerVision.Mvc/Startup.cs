using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ComputerVision.Mvc.Startup))]
namespace ComputerVision.Mvc
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
