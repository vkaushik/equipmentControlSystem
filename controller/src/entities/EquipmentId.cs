using System;

namespace EquipmentControlSystem.Controller {
    public enum EquipmentType { Light, AirConditioner, Sensor }
    public class EquipmentId {
        public static readonly char idSeparator = '^';
        public string value { get; }
        public string floorId { get; }
        public string corridorId { get; }
        public EquipmentType type { get; }
        public string sequenceId { get; }

        public EquipmentId (string floorId, string corridorId, EquipmentType type, string sequenceId) {
            this.floorId = floorId;
            this.corridorId = $"{floorId}{idSeparator}{corridorId}";
            this.type = type;
            this.sequenceId = sequenceId;
            string[] keys = { floorId, corridorId, type.ToString (), sequenceId };
            this.value = string.Join (idSeparator.ToString (), keys);
        }
    }
}