namespace Kulaszka.Collections
{
    public class QueueIsClosedException : Exception
    {
        public QueueIsClosedException()
            : base("Queue is closed.")
        {
        }
    }
}
