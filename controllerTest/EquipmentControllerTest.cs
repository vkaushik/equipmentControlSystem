using System;
using System.Collections.Generic;
using System.Text.Json;
using EquipmentControlSystem.CommandParser;
using EquipmentControlSystem.Controller;
using Xunit;

namespace EquipmentControlSystem.ControllerTest {
    public class EquipmentControllerTest {
        [Theory]
        [TestData]
        public void ShouldProcessTheCommandsAsExpected (int serial, ControllerConfig config, List<string> commands) {
            log ($"******** start test #{serial} **********");
            var equipmentController = EquipmentControllerFactory.CreateControllerUsing (config, log);

            var options = new JsonSerializerOptions {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            log (JsonSerializer.Serialize<ControllerConfig> (config, options));
            log ("");

            equipmentController.LogStatus ();

            foreach (var command in commands) {
                log ($"Input: { command }");
                log ("");

                (bool isValidCommand, Sensor sensor, Signal signal) =
                Command.Parse (command, equipmentController);

                if (isValidCommand) {
                    sensor.send (signal);
                } else {
                    log ("Invalid command");
                }
            }
            log ($"******** end test #{serial} **********");
            log ("");
        }

        internal static Action<string> log = (string line) => {
            Console.WriteLine (line);
        };
    }
}