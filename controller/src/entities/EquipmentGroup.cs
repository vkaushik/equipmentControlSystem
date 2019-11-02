using System;
using System.Collections;
using System.Collections.Generic;

namespace EquipmentControlSystem.Controller {
    public class EquipmentGroup : IEnumerable<Equipment> {
        private readonly HashSet<Equipment> _equipments = new HashSet<Equipment> ();
        public float maximumPowerConsmption { get; private set; }
        public float presentPowerConsumption {
            get {
                float power = 0;
                foreach (var equipment in _equipments) {
                    if (equipment.isOn) {
                        power += equipment.power;
                    }
                }
                return power;
            }
        }

        public void SwitchOnAll () {
            foreach (var equipment in _equipments) {
                equipment.switchOn ();
            }
        }

        public void SwitchOffAll () {
            foreach (var equipment in _equipments) {
                equipment.switchOff ();
            }
        }

        public IEnumerator<Equipment> GetEnumerator () {
            return this._equipments.GetEnumerator ();
        }

        IEnumerator IEnumerable.GetEnumerator () {
            return (IEnumerator) GetEnumerator ();
        }

        internal void Add (Equipment equipment) {
            this._equipments.Add (equipment);
            this.maximumPowerConsmption += equipment.power;
        }
    }

}