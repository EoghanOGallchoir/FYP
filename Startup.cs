using Microsoft.Owin;
using Owin;
[assembly: OwinStartup(typeof(FYP.Startup))]
//[assembly: OwinStartupAttribute(typeof(FYP.Startup))]
namespace FYP
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
           // ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
