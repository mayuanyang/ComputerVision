using Autofac;
using Enexure.MicroBus.Autofac;

namespace ComputerVision.Core
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterMicroBus(MicroBusRegistration.GetBusBuilder());
        }
    }
}
