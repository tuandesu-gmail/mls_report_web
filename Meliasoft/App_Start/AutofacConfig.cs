using Autofac;
using Autofac.Integration.Mvc;
using Meliasoft.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Meliasoft
{
    public class AutofacConfig
    {
        public static void RegisterContainers()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterFilterProvider();

            builder.RegisterType<MeliasoftData>().As<IMeliasoftData>().InstancePerHttpRequest();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(builder.Build()));
        }
    }
}