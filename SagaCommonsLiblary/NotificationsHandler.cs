using System;
using MassTransit;

namespace SagaCommonsLibrary
{
    public class NotificationsHandler : Consumes<IAuctionStartedNotification>.Selected, Consumes<IAuctionEndedNotification>.Selected, Consumes<IAuctionStatusUpdate>.Selected
    {
        public Guid CorrelationId { get; set; }
        public event EventHandler AuctionGuidWasRecieved;
        public event EventHandler AuctionStatusWasRecieved;

        public virtual void Consume(IAuctionStartedNotification auctionStartedMessage)
        {
            if (auctionStartedMessage == null) return;

            CorrelationId = auctionStartedMessage.CorrelationId;
            OnAuctionGuidWasRecieved();
            Console.WriteLine("Recieved message of type {0}", auctionStartedMessage.GetType());
            Console.WriteLine("An auction has just started!");
            Console.WriteLine("Notification from: {0}", auctionStartedMessage.Sender);
            Console.WriteLine("Notification text: {0}", auctionStartedMessage.NotificationText);
        }

        public virtual void Consume(IAuctionEndedNotification auctionEndedMessage)
        {
            if (auctionEndedMessage == null) return;

            Console.WriteLine("Recieved message of type {0}", auctionEndedMessage.GetType());
            Console.WriteLine("An auction has just ended!");
            Console.WriteLine("Notification from: {0}", auctionEndedMessage.Sender);
            Console.WriteLine("Notification text: {0}", auctionEndedMessage.NotificationText);
            Console.WriteLine("Auction winner: {0}", auctionEndedMessage.WinningBidder);
            Console.WriteLine("Auction winning bid: {0}", auctionEndedMessage.WinningBid);
        }

        public virtual void Consume(IAuctionStatusUpdate message)
        {
            if (message == null) return;

            Console.WriteLine("Recieved message of type {0}", message.GetType());
            Console.WriteLine("New bid was placed!");
            Console.WriteLine("Notification from: {0}", message.Sender);
            Console.WriteLine("Current Auction winner: {0}", message.WinningBidder);
            Console.WriteLine("Current Auction winning bid: {0}", message.WinningBid);
            OnAuctionStatusWasRecieved(message.WinningBid, message.WinningBidder);
        }

        public virtual bool Accept(IAuctionStartedNotification message)
        {
            return message != null && (message.CorrelationId.Equals(CorrelationId) || CorrelationId.Equals(Guid.Empty));
        }

        public virtual bool Accept(IAuctionEndedNotification message)
        {
            return message != null && (message.CorrelationId.Equals(CorrelationId) || CorrelationId.Equals(Guid.Empty));
        }
        public bool Accept(IAuctionStatusUpdate message)
        {
            return message != null && (message.CorrelationId.Equals(CorrelationId) || CorrelationId.Equals(Guid.Empty));
        }
        protected virtual void OnAuctionGuidWasRecieved()
        {
            var handler = AuctionGuidWasRecieved;
            if (handler != null) handler(this, new AuctionGuidWasRecievedEventArgs {GuidRecieved = CorrelationId});
        }
        protected virtual void OnAuctionStatusWasRecieved(decimal winningBid, string winningBidder)
        {
            var handler = AuctionStatusWasRecieved;
            if (handler != null) handler(this, new AuctionStatusWasRecievedEvent { WinningBid = winningBid, WinningBidder = winningBidder});
        }
    }

    public class AuctionGuidWasRecievedEventArgs : EventArgs
    {
        public Guid GuidRecieved { get; set; }
    }

    public class AuctionStatusWasRecievedEvent : EventArgs
    {
        public decimal WinningBid { get; set; }
        public string WinningBidder { get; set; }
    }
}
