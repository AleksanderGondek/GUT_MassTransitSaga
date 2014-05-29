using System;
using SagaCommonsLibrary;

namespace AuctionController
{
    class Program
    {
        private const string ProcessName = "AuctionController";
        private const string ActionToPerformName = "start controlling auction";
        private static Guid _auctionGuid = Guid.Empty;

        static void Main()
        {
            _auctionGuid = Guid.NewGuid();
            
            var controllerIncomingMessageHandler = new NotificationsHandler();
            
            var controller = new ControllerUser
            {
                CorrelationGuid = _auctionGuid,
                ProcessName = ProcessName,
                ActionToPerformName = ActionToPerformName,
                MessagesHandler = controllerIncomingMessageHandler
            };

            controller.Bus = controller.InitializeBus();
            controllerIncomingMessageHandler.AuctionGuidWasRecieved += controller.HandleAuctionGuidWasRecievedEvent;
            controllerIncomingMessageHandler.AuctionStatusWasRecieved += controller.HandleAuctionStatusWasRecievedEvent;

            controller.Run();
        }
    }
}
