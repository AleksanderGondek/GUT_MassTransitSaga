using System;
using SagaCommonsLibrary;

namespace AuctionController
{
    public class ControllerUser: AuctionHouseUser
    {
        private bool AuctionIsInProgress { get; set; }

        public ControllerUser()
        {
            AuctionIsInProgress = false;
        }

        public override sealed void DoWork()
        {
            try
            {
                Console.WriteLine(AuctionIsInProgress ? "Press 'E' to end auction" : "Press 'S' to start auction");
                var key = Console.ReadKey();

                if (key.Key.Equals(ConsoleKey.S))
                {
                    var messageToBeSent = new StartAuctionMessage {CorrelationId = CorrelationGuid, Sender = ProcessName, ShouldStartAuction = true};
                    AuctionIsInProgress = true;
                    Bus.Publish(messageToBeSent);
                }
                
                if(key.Key.Equals(ConsoleKey.E))
                {
                    var messageToBeSent = new EndAuctionMessage { CorrelationId = CorrelationGuid, Sender = ProcessName, ShouldEndAuction = true };
                    AuctionIsInProgress = false;
                    Bus.Publish(messageToBeSent);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error while sending message! Error: {0}", exception.Message);
            }
        }
    }
}
