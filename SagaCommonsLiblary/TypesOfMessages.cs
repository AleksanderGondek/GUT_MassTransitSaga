using System;
using MassTransit;

namespace SagaCommonsLibrary
{
    public interface INotification : CorrelatedBy<Guid>
    {
        string Sender { get; set; }
    }

    public interface IStartAuction : INotification
    {
        bool ShouldStartAuction { get; set; }
    }

    public interface IEndAuction : INotification
    {
        bool ShouldEndAuction { get; set; }
    }

    public interface IAuctionStartedNotification : INotification
    {
        string NotificationText { get; set; }
    }

    public interface IAuctionEndedNotification : INotification
    {
        string NotificationText { get; set; }
        decimal WinningBid { get; set; }
        string WinningBidder { get; set; }
    }

    public interface IAuctionBid : INotification
    {
        decimal Bid { get; set; }
        string Bidder { get; set; }        
    }

    public interface IAuctionStatusUpdate : INotification
    {
        decimal WinningBid { get; set; }
        string WinningBidder { get; set; }
    }
}
