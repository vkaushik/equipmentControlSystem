using System;

namespace EquipmentControlSystem.Controller {
    public class Equipment {
        private bool _isOn;
        public EquipmentId id { get; }
        public float power { get; }
        public bool isOn { get => this._isOn; }

        public Equipment (EquipmentId id, float power) {
            this.id = id;
            this.power = power;
            this._isOn = false;
        }

        public void switchOn () {
            this._isOn = true;
        }

        public void switchOff () {
            this._isOn = false;
        }
    }
}