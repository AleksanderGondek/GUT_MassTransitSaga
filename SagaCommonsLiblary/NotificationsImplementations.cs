using System;

namespace SagaCommonsLibrary
{
    public class StartAuctionMessage : IStartAuction
    {
        public Guid CorrelationId { get; set; }
        public bool ShouldStartAuction { get; set; }
        public string Sender { get; set; }
    }

    public class EndAuctionMessage : IEndAuction
    {
        public Guid CorrelationId { get; set; }
        public bool ShouldEndAuction { get; set; }
        public string Sender { get; set; }
    }

    public class AuctionStartNotification : IAuctionStartedNotification
    {
        public Guid CorrelationId { get; set; }
        public string Sender { get; set; }
        public string NotificationText { get; set; }
    }

    public class AuctionEndedNotification : IAuctionEndedNotification
    {
        public Guid CorrelationId { get; set; }
        public string Sender { get; set; }
        public string NotificationText { get; set; }
        public decimal WinningBid { get; set; }
        public string WinningBidder { get; set; }
    }

    public class AuctionBidMessages : IAuctionBid
    {
        public Guid CorrelationId { get; set; }
        public string Sender { get; set; }
        public decimal Bid { get; set; }
        public string Bidder { get; set; }
    }

    public class AuctionStatusUpdateMessage : IAuctionStatusUpdate
    {
        public Guid CorrelationId { get; set; }
        public string Sender { get; set; }
        public decimal WinningBid { get; set; }
        public string WinningBidder { get; set; }
    }
}
