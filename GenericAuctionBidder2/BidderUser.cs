using System;
using SagaCommonsLibrary;

namespace GenericAuctionBidder2
{
    public class BidderUser : AuctionHouseUser
    {
        private bool AuctionIsInProgress { get; set; }
        private Random Random { get; set; }

        public BidderUser()
        {
            AuctionIsInProgress = false;
            Random = new Random(DateTime.Now.Second + DateTime.Now.Millisecond);
        }

        public override void HandleAuctionGuidWasRecievedEvent(object sender, EventArgs args)
        {
            base.HandleAuctionGuidWasRecievedEvent(sender, args);
            AuctionIsInProgress = true;
        }

        public override sealed void DoWork()
        {
            if (AuctionIsInProgress)
            {
                Console.WriteLine("Press any key to send new bid offer");
                Console.ReadLine();

                var newBid = new AuctionBidMessages
                {
                    CorrelationId = CorrelationGuid,
                    Bid =
                        CurrentWinningBid == 0
                            ? Random.Next(1000)
                            : (CurrentWinningBid +
                               Convert.ToDecimal(Convert.ToDouble(CurrentWinningBid) * Random.NextDouble())),
                    Bidder = ProcessName,
                    Sender = ProcessName
                };

                Bus.Publish(newBid);
                Console.WriteLine("Bid sent");
            }
            else
            {
                Console.WriteLine("Waiting for auction to start");
            }
        }
    }
}
