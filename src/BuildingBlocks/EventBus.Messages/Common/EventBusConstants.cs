namespace EventBus.Messages.Common
{
    public static class EventBusConstants
    {
        //this is constant and should not change like a config value, that's why we set it here and not in appsettings.json for instance
        //this queue name will be shown in rabbitmq management system in our docker image
        public const string BasketCheckoutQueue = "basketcheckout-queue";
    }
}
