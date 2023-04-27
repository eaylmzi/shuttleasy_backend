using System.Collections;

namespace shuttleasy.Services.NotifService.ObserverPattern
{
    public class ConcreteSubject : ISubject
    {
        private ArrayList observers = new ArrayList();

        public void Register(IObserver o)
        {
            observers.Add(o);
        }

        public void Detach(IObserver o)
        {
            observers.Remove(o);
        }

        public void Notify()
        {
            foreach (IObserver o in observers)
            {
               // o.Update();
            }
        }
    }
}
