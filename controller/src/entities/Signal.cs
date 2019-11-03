using System;

namespace EquipmentControlSystem.Controller {
    public enum SingalType { unknown, movement, noMovement }
    public class Signal {
        public SingalType type { get; }
        public Sensor source { get; }
        public Signal (Sensor source, SingalType type) {
            this.type = type;
            this.source = source;
        }
    }
}