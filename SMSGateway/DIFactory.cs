using Autofac;
using Autofac.Core;
using SMSGateway.Interface;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace SMSGateway
{
    public class DIFactory : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SMSSender>().As<ISMSSender>();
            builder.RegisterType<CustomDependencyResolver>().As<ICustomDependencyResolver>();
            builder.RegisterType<SMSData>().As<ISMSData>();
            builder.RegisterType<DataSet>();
            builder.RegisterType<SqlCommand>();
            builder.RegisterType<SqlConnection>();
            builder.RegisterType<StreamReader>();
            builder.RegisterType<Task>();
            builder.RegisterType<SMSResponse>();
            builder.RegisterType<Logger>().As<ILogger>();
            builder.RegisterType<ACLGateway>();
            builder.RegisterType<SBIGateway>();
            builder.RegisterType<SqlManager>();

        }

        public interface ICustomDependencyResolver
        {
            TResolved Resolve<TResolved>();
            TResolved ResolveNamed<TResolved>(string Name);
            TResolved ResolveKeyed<TResolved>(string Key);
            TResolved ResolveParameter<TResolved>(Parameter[] prm);
        }

        internal class CustomDependencyResolver : ICustomDependencyResolver
        {
            private readonly ILifetimeScope _lifetimeScope;

            public CustomDependencyResolver(ILifetimeScope lifetimeScope)
            {
                _lifetimeScope = lifetimeScope;
            }

            public TResolved Resolve<TResolved>()
                => _lifetimeScope.Resolve<TResolved>();

            public TResolved ResolveNamed<TResolved>(string Name)
               => _lifetimeScope.ResolveNamed<TResolved>(Name);
            public TResolved ResolveKeyed<TResolved>(string Key)
               => _lifetimeScope.ResolveKeyed<TResolved>(Key);

            public TResolved ResolveParameter<TResolved>(Parameter[] prm)
               => _lifetimeScope.Resolve<TResolved>(prm);

        }
    }
}
