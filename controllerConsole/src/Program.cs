using System;
using EquipmentControlSystem.CommandParser;
using EquipmentControlSystem.Controller;

namespace EquipmentControlSystem.ControllerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            log("Starting...");

            var config = new ControllerConfig(2, 1, 2, 1, 1, 1, 1, 5, 10);

            string[] commands = {
                "Movement in Floor 1, Sub corridor 2",
                "No movement in Floor 1, Sub corridor 2 for a minute",
            };

            var equipmentController = EquipmentControllerFactory.CreateControllerUsing(config, log);
            equipmentController.LogStatus();

            foreach (var command in commands)
            {
                log($"Input: { command }");
                log("");
                if (Command.TryParse(command, out Sensor sensor, out Signal signal, equipmentController, log))
                {
                    sensor.send(signal);
                    equipmentController.LogStatus();
                }
            }

        }
        private static Action<string> log = (string line) =>
        {
            Console.WriteLine(line);
        };
    }
}