using System;
using System.Collections.Generic;

namespace EquipmentControlSystem.Controller {
    public interface IEquipmentController : IObserver<Signal> {
        void ActivateNightSlot ();
        void InstallMainCorridorEquipment (Equipment equipment);
        void InstallSubCorridorEquipment (Equipment equipment);
        void InstallSubCorridorEquipmentWithSensor (Equipment equipment, Sensor sensor);
        Sensor GetSensorById (EquipmentId sensorId);
        void LogStatus ();

        // IObserver methods
        new void OnCompleted ();
        new void OnError (Exception error);
        new void OnNext (Signal signal);
        void Subscribe (IObservable<Signal> provider);
        void Unsubscribe ();
    }
}