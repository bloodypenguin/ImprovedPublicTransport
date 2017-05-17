namespace ImprovedPublicTransport
{
    public struct ItemClassTriplet
    {

        public ItemClassTriplet(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level)
        {
            this.Service = service;
            this.SubService = subService;
            this.Level = level;
        }


        public ItemClass.Service Service { get; }
        public ItemClass.SubService SubService { get; }
        public ItemClass.Level Level { get; }


    }
}