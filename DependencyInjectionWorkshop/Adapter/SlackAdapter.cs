namespace DependencyInjectionWorkshop.Models
{
    using SlackAPI;

    public class SlackAdapter
    {
        public void NotifyAuthFailed()
        {
            var slackClient = new SlackClient("my api token");
            slackClient.PostMessage(slackResponse => { }, "my channel", "my message", "my bot name");
        }
    }
}