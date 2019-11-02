using System;
using System.Text.Json;
using Xunit;
using EquipmentControlSystem.Controller;
using System.Collections.Generic;
using EquipmentControlSystem.CommandParser;

namespace EquipmentControlSystem.ControllerTest
{
    public class EquipmentControllerTest
    {
        [Theory]
        [TestData]
        public void ShouldProcessTheCommandsAsExpected(int serial, ControllerConfig config, List<string> commands)
        {
            log($"******** start test #{serial} **********");
            var equipmentController = EquipmentControllerFactory.CreateControllerUsing(config, log);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            log(JsonSerializer.Serialize<ControllerConfig>(config, options));
            log("");
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
            log($"******** end test #{serial} **********");
            log("");
        }

        internal static Action<string> log = (string line) =>
        {
            Console.WriteLine(line);
        };
    }
}
