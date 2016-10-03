using Enexure.MicroBus;

namespace ComputerVision.Core
{
    class MicroBusRegistration
    {
        public static BusBuilder GetBusBuilder()
        {
            return new BusBuilder().RegisterHandlers(CoreAssembly.Assembly);
        }
    }
}
