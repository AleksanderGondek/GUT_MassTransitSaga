using System;
using System.Linq;
using SagaCommonsLibrary;

namespace GenericAuctionBidder2
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!args.Any())
            {
                args = new[] { "BidderTwo", "send new bid" };
            }

            var bidder = new BidderUser
            {
                ProcessName = args[0],
                ActionToPerformName = args[1],
                Bus = null,
                MessagesHandler = null
            };

            var bidHandler = new NotificationsHandler
            {
                CorrelationId = Guid.Empty
            };

            bidder.MessagesHandler = bidHandler;
            bidder.Bus = bidder.InitializeBus();

            bidHandler.AuctionGuidWasRecieved += bidder.HandleAuctionGuidWasRecievedEvent;
            bidHandler.AuctionStatusWasRecieved += bidder.HandleAuctionStatusWasRecievedEvent;

            bidder.Run();
        }
    }
}
