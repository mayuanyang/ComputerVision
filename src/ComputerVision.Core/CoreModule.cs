using Autofac;
using Mediator.Net;
using Mediator.Net.Autofac;


namespace ComputerVision.Core
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            var mediatorBuilder = new MediatorBuilder();
            mediatorBuilder.RegisterHandlers(this.GetType().Assembly);
            builder.RegisterMediator(mediatorBuilder);
        }
    }
}
