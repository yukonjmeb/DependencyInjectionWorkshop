namespace DependencyInjectionWorkshop.Models
{
    using SlackAPI;

    public interface INotification
    {
        void SlackAdapter();
    }

    public class Notification : INotification
    {
        public void SlackAdapter()
        {
            var slackClient = new SlackClient("my api token");
            slackClient.PostMessage(slackResponse => { }, "my channel", "my message", "my bot name");
        }
    }
}