namespace Persistence
{
    public class Publisher
    {
        public uint PublisherID { get; set; }
        public string PublisherName { get; set; }
        public Publisher()
        {
            
        }
        public Publisher(uint publisherID, string publisherName)
        {
            PublisherID = publisherID;
            PublisherName = publisherName;
        }
    }
}