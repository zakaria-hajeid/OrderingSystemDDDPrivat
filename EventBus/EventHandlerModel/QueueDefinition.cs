namespace EventBus.EventHandlerModel
{
    public class QueueDefinition
    {
        public string Name { get; set; }
        public string RoutingKey { get; set; }
        public string Exchange { get; set; }
        public string type { get; set; }
    }
}
