using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(solution_MVC_Arthouse.Startup))]
namespace solution_MVC_Arthouse
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
