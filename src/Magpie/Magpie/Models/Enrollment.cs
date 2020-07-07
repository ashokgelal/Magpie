namespace MagpieUpdater.Models
{
    public class Enrollment
    {
        public Channel Channel { get; private set; }
        public bool IsRequired { get; set; }
        public bool IsEnrolled { get; set; }
        public string Email { get; set; }

        public Enrollment(Channel channel)
        {
            Channel = channel;
        }
    }
}