
using MassTransit;
using MassTransit.Saga;
using SagaCommonsLibrary;

namespace Saga
{
    public class MassTranistAuctionHouseUser : AuctionHouseUser
    {
        public override IServiceBus InitializeBus()
        {
            return ServiceBusFactory.New(init =>
            {
                init.UseMsmq(cfg =>
                {
                    cfg.VerifyMsmqConfiguration();
                    cfg.UseMulticastSubscriptionClient();
                });
                init.ReceiveFrom("msmq://localhost/" + ProcessName.Trim().ToLower());
                init.Subscribe(subscribtion => subscribtion.Saga(new InMemorySagaRepository<AuctionHouse>()));
            });
        }
    }
}
