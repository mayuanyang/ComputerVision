using System;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using ComputerVision.Core;
using ConfigInjector.Configuration;


namespace ComputerVision.Mvc
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            string pathValue = Environment.GetEnvironmentVariable("PATH");
            string domainBaseDir = AppDomain.CurrentDomain.BaseDirectory;
            string cntkPath = domainBaseDir + @"bin\";
            pathValue += ";" + cntkPath;
            Environment.SetEnvironmentVariable("PATH", pathValue);

            var builder = new ContainerBuilder();

            ConfigurationConfigurator.RegisterConfigurationSettings()
                .FromAssemblies(CoreAssembly.Assembly)
                .RegisterWithContainer(configSetting => builder.RegisterInstance(configSetting).AsSelf().SingleInstance())
                .ExcludeSettingKeys(new[] {
                    "aspnet:PortableCompilationOutput",
                    "aspnet:PortableCompilationOutputSnapshotType",
                    "aspnet:DisableFcnDaclRead",
                    "WEBSITE_AUTH_ENABLED",
                    "ScmType",
                    "WEBSITE_SITE_NAME",
                    "REMOTEDEBUGGINGVERSION",
                    "APPINSIGHTS_INSTRUMENTATIONKEY",
                    "WEBSITE_NODE_DEFAULT_VERSION",
                    "serilog:minimum-level",
                    "webpages:Version",
                    "webpages:Enabled",
                    "ClientValidationEnabled",
                    "UnobtrusiveJavaScriptEnabled",
                "autoFormsAuthentication",
                "enableSimpleMembership"})
                .DoYourThing();

            builder.RegisterModule<CoreModule>();
            
            // Register MVC controllers
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            
            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            container.BeginLifetimeScope();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}
