using System;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;
using SagaCommonsLibrary;

namespace Saga
{
    public class AuctionHouse : SagaStateMachine<AuctionHouse>, ISaga
    {
        public Guid CorrelationId { get; set; }
        public IServiceBus Bus { get; set; }

        private decimal CurrentHighestBid { get; set; }
        private string CurrentHighestBidder { get; set; }

        public static State Initial { get; set; }
        public static State Completed { get; set; }
        public static State InProgress { get; set; }

        public static Event<IStartAuction> StartAuction { get; set; }
        public static Event<IAuctionBid> AuctionBid { get; set; }
        public static Event<IEndAuction> EndAuction { get; set; }

        public AuctionHouse(Guid guid)
        {
            CorrelationId = guid;
        }

        static AuctionHouse() 
        {
            Define(() => {
                Initially(When(StartAuction)
                    .Then(saga => saga.OnStartAuction())
                    .TransitionTo(InProgress));
                During(InProgress,
                    When(EndAuction)
                    .Then(saga => saga.OnEndAuction())
                    .Complete());
                During(InProgress,
                    When(AuctionBid)
                    .Then(saga => saga.OnAuctionBid())
                    .TransitionTo(InProgress));   
            });
        }

        private void OnAuctionBid()
        {
            var recievedMessage = Bus.MessageContext<IAuctionBid>().Message;
            if (recievedMessage == null) return;

            Console.WriteLine("Bid request recieved !");
            Console.WriteLine("Processing bid!");
            
            if (recievedMessage.Bid <= CurrentHighestBid || string.IsNullOrEmpty(recievedMessage.Bidder))
            {
                Console.WriteLine("Bad bid. Not accepted");
                return;
            }

            CurrentHighestBid = recievedMessage.Bid;
            CurrentHighestBidder = recievedMessage.Bidder;

            Console.WriteLine("New winner is: {0}", CurrentHighestBidder);
            Console.WriteLine("New winning bet is: {0}", CurrentHighestBid);
            Console.WriteLine("Sending out bid update!");

            var newBidUpdateMessage = new AuctionStatusUpdateMessage
            {
                CorrelationId = CorrelationId,
                Sender = "Auction House",
                WinningBid = CurrentHighestBid,
                WinningBidder = CurrentHighestBidder
            };

            Bus.Publish(newBidUpdateMessage);
            Console.WriteLine("New bid notifications sent!");
        }

        private void OnEndAuction()
        {
            Console.WriteLine("End auction request recieved !");
            Console.WriteLine("Ending auction!");
            Console.WriteLine("Auction winner is: {0}", CurrentHighestBidder);
            Console.WriteLine("Auction winning bet is: {0}", CurrentHighestBid);
            Console.WriteLine("Sending out end of auction notifications!");

            var endOfAuctionNotification = new AuctionEndedNotification
            {
                CorrelationId = CorrelationId,
                Sender = "Auction House",
                NotificationText = "Auction has ended!",
                WinningBid = CurrentHighestBid,
                WinningBidder = CurrentHighestBidder
            };

            Bus.Publish(endOfAuctionNotification);
            Console.WriteLine("End of auction notifications sent!");
        }

        private void OnStartAuction()
        {
            var recievedMessage = Bus.MessageContext<IStartAuction>().Message;
            if (recievedMessage == null) return;

            Console.WriteLine("Recieved new auction request !");
            CorrelationId = recievedMessage.CorrelationId;
            CurrentHighestBid = 0;
            CurrentHighestBidder = String.Empty;

            Console.WriteLine("New auction started! Sending out notifications!");
            var newAuctionNotificationMessage = new AuctionStartNotification
            {
                CorrelationId = CorrelationId,
                NotificationText = "New Auction! We are selling X",
                Sender = "Auction House"
            };

            Bus.Publish(newAuctionNotificationMessage);
            Console.WriteLine("New auction notifications sent!");
        }
    }
}
