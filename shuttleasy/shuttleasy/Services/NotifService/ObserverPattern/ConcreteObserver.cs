namespace shuttleasy.Services.NotifService.ObserverPattern
{
    public class ConcreteObserver : IObserver
    {
        private string name;
        private string channelName;

        public ConcreteObserver(string name)
        {
            this.name = name;
        }

        public string getName()
        {
            return name;
        }

        public void setName(string name)
        {
            this.name = name;
        }

        public string getChannelName()
        {
            return channelName;
        }

        public void setChannelName(string channelName)
        {
            this.channelName = channelName;
        }
    }
}
