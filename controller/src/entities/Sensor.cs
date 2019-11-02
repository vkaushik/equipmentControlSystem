using System;
using System.Collections.Generic;

namespace EquipmentControlSystem.Controller {
    public class Sensor : IObservable<Signal> {
        List<IObserver<Signal>> _observers;
        public EquipmentId id { get; }
        public Sensor (EquipmentId id) {
            this._observers = new List<IObserver<Signal>> ();
            this.id = id;
        }

        public IDisposable Subscribe (IObserver<Signal> observer) {
            if (!_observers.Contains (observer)) {
                _observers.Add (observer);
            }

            return new Unsubscriber (_observers, observer);
        }

        public void send (Signal signal) {
            foreach (var observer in _observers) {
                observer.OnNext (signal);
            }
        }

        private class Unsubscriber : IDisposable {
            private List<IObserver<Signal>> _observers;
            private IObserver<Signal> _observer;

            public Unsubscriber (List<IObserver<Signal>> observers, IObserver<Signal> observer) {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose () {
                if (_observer != null) {
                    _observers.Remove (_observer);
                };
            }
        }
    }
}