using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jTech.DesignPatterns.Observer
{
    public interface IObserverSubject<T> where T : EventArgs
    {
        void Subscribe(EventHandler<T> handler);
        void Unsubscribe(EventHandler<T> handler);
    }
}
