using System;
using EquipmentControlSystem.Controller;

namespace EquipmentControlSystem.CommandParser
{
    public class Command
    {
        public static bool TryParse(string command, out Sensor sensor, out Signal signal,
            IEquipmentController controller, Action<string> log)
        {
            sensor = null;
            signal = null;
            var words = command.Split(' ');

            string floorId;
            string subCorridorId;
            string sequenceId = "1";

            if (words.Length == 7 && command.Contains("Movement"))
            {
                floorId = words[3][0].ToString();
                subCorridorId = words[6];

                var sensorId = new EquipmentId(floorId, $"SC-{subCorridorId}",
                    EquipmentType.Sensor, sequenceId);

                foreach (var subscribedSensor in controller.Sensors)
                {
                    if (subscribedSensor.id.value == sensorId.value)
                    {
                        sensor = subscribedSensor;
                        signal = new Signal(sensor, SingalType.movement);
                        return true;
                    }
                }
            }
            else if (words.Length == 11 & command.Contains("No movement"))
            {
                floorId = words[4][0].ToString();
                subCorridorId = words[7];

                var sensorId = new EquipmentId(floorId, $"SC-{subCorridorId}",
                    EquipmentType.Sensor, sequenceId);

                foreach (var subscribedSensor in controller.Sensors)
                {
                    if (subscribedSensor.id.value == sensorId.value)
                    {
                        sensor = subscribedSensor;
                        signal = new Signal(sensor, SingalType.noMovement);
                        return true;
                    }
                }
            }
            else
            {
                log("Invalid command");
                return false;
            }
            log("Something went wrong");
            return false;
        }
    }
}

/* 
examples:
    Movement in Floor 1, Sub corridor 2
    No movement in Floor 1, Sub corridor 2 for a minute
*/
