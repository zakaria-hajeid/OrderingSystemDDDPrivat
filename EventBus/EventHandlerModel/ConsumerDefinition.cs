namespace EventBus.EventHandlerModel
{
    public class ConsumerDefinition
    {
        public bool Active { get; set; }
        public int[] Channels { get; set; }
        public string Exchange { get; set; }
        public string type { get; set; }

    }
}
