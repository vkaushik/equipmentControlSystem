using System;
using EquipmentControlSystem.Controller;

namespace EquipmentControlSystem.CommandParser {
    public class Command {
        public static (bool, Sensor, Signal) Parse (string command, IEquipmentController controller) {
            Sensor sensor = null;
            Signal signal = null;
            var words = command.Split (' ');

            string floorId = "";
            string subCorridorId = "";
            string sequenceId = "1";

            // Hard-coded rules to parse command.
            if (words.Length == 7 && command.Contains ("Movement")) {
                floorId = words[3][0].ToString ();
                subCorridorId = words[6];
            } else if (words.Length == 11 & command.Contains ("No movement")) {
                floorId = words[4][0].ToString ();
                subCorridorId = words[7];
            }

            var sensorId = new EquipmentId (floorId, $"SC-{subCorridorId}",
                EquipmentType.Sensor, sequenceId);

            sensor = controller.GetSensorById (sensorId);

            if (sensor is null) {
                return (false, sensor, signal);
            }

            signal = new Signal (sensor, SingalType.movement);

            return (true, sensor, signal);
        }
    }
}