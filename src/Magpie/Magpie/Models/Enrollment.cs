namespace MagpieUpdater.Models
{
    public class Enrollment
    {
        public Channel Channel { get; private set; }
        public bool IsRequired { get; internal set; }
        public bool IsEnrolled { get; internal set; }
        public string Email { get; internal set; }

        public Enrollment(Channel channel)
        {
            Channel = channel;
        }
    }
}