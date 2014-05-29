using System;
using MassTransit;
using MassTransit.Exceptions;

namespace SagaCommonsLibrary
{
    public abstract class AuctionHouseUser
    {
        public Guid CorrelationGuid { get; set; }
        public IServiceBus Bus { get; set; }
        public string ProcessName { get; set; }
        public string ActionToPerformName { get; set; }
        public NotificationsHandler MessagesHandler { get; set; }

        protected decimal CurrentWinningBid { get; set; }
        protected string CurrentWinningBidder { get; set; }

        protected AuctionHouseUser()
        {
            CurrentWinningBid = 0.0m;
            CurrentWinningBidder = string.Empty;
        }

        public void Run()
        {
            // We need to remember about setting up varables
            if (Bus == null || string.IsNullOrEmpty(ProcessName) || String.IsNullOrEmpty(ActionToPerformName)) throw new NotImplementedByDesignException();
            Console.Title = ProcessName;
            Console.WriteLine("Service {0} is starting up!", ProcessName);
            Console.WriteLine("Calling IServiceBus Probe method to make sure bus is working.");
            Bus.Probe();
            Console.WriteLine("Probing called successfully");

            ConsoleKeyInfo readKey;
            do
            {
                Console.WriteLine("Press any key to {0}", ActionToPerformName);
                Console.WriteLine("Press ESC to terminate");
                readKey = Console.ReadKey();
                DoWork();

            } while (!readKey.Key.Equals(ConsoleKey.Escape));
        }

        public virtual IServiceBus InitializeBus()
        {
            return ServiceBusFactory.New(init =>
            {
                init.UseMsmq(cfg =>
                {
                    cfg.VerifyMsmqConfiguration();
                    cfg.UseMulticastSubscriptionClient();
                });
                init.ReceiveFrom("msmq://localhost/"+ProcessName.Trim().ToLower());
                init.Subscribe(subscribtion => subscribtion.Instance(MessagesHandler));
            });
        }

        public virtual void DoWork()
        {
            Console.WriteLine(string.Empty);
        }

        public virtual void HandleAuctionGuidWasRecievedEvent(object sender, EventArgs args)
        {
            var newCorrelationIdHolder = args as AuctionGuidWasRecievedEventArgs;
            if (newCorrelationIdHolder == null) return;

            CorrelationGuid = newCorrelationIdHolder.GuidRecieved;
        }

        public virtual void HandleAuctionStatusWasRecievedEvent(object sender, EventArgs args)
        {
            var newBidHolder = args as AuctionStatusWasRecievedEvent;
            if (newBidHolder == null) return;

            CurrentWinningBid = newBidHolder.WinningBid;
            CurrentWinningBidder = newBidHolder.WinningBidder;
        }
    }
}
