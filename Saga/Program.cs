namespace Saga
{
    class Program
    {
        private const string ProcessName = "Auction House";
        private const string ActionToPerformName = "Do nothing, cause this is auction house";
        static void Main()
        {
            var auctionHouse = new MassTranistAuctionHouseUser
            {
                ProcessName = ProcessName,
                ActionToPerformName = ActionToPerformName,
                MessagesHandler = null
            };

            auctionHouse.Bus = auctionHouse.InitializeBus();
            auctionHouse.Run();
        }
    }
}
