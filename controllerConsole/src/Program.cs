using System;
using EquipmentControlSystem.CommandParser;
using EquipmentControlSystem.Controller;

namespace EquipmentControlSystem.ControllerConsole {
    class Program {
        static void Main (string[] args) {
            log ("Starting Equipment Control System...");

            ControllerConfig config = GetControllerConfiguration (log);

            log ("");
            log ("Creating and installing equipments...");
            var equipmentController = EquipmentControllerFactory.CreateControllerUsing (config, log);

            log ("(Please kill the application to exit.)");
            log ("");
            log ("Command examples: ");
            log ("Command: Movement in Floor 1, Sub corridor 2");
            log ("Command: No movement in Floor 1, Sub corridor 2 for a minute");
            log ("");

            while (true) {
                equipmentController.LogStatus ();
                log ("---------------------------------------------------");
                var command = input ("Command: ");
                log ("");
                Process (command, equipmentController);
            }
        }

        private static void Process (string command, IEquipmentController controller) {
            (bool isValidCommand, Sensor sensor, Signal signal) = Command.Parse (command, controller);

            if (isValidCommand) {
                sensor.send (signal);
            } else {
                log ("Warning: Invalid command");
                log ("");
            }
        }

        private static ControllerConfig GetControllerConfiguration (Action<string> log) {
            log ("");
            log ("Enter configuration");
            int numberOfFloors = inputNumber ("numberOfFloors: ");
            int numberOfMainCorridorsPerFloor = inputNumber ("numberOfMainCorridorsPerFloor: ");
            int numberOfSubCorridorsPerFloor = inputNumber ("numberOfSubCorridorsPerFloor: ");
            int numberOfLightsPerMainCorridor = inputNumber ("numberOfLightsPerMainCorridor: ");
            int numberOfLightsPerSubCorridor = inputNumber ("numberOfLightsPerSubCorridor: ");
            int numberOfAirConditionersPerMainCorridor = inputNumber ("numberOfAirConditionersPerMainCorridor: ");
            int numberOfAirConditionersPerSubCorridor = inputNumber ("numberOfAirConditionersPerSubCorridor: ");
            int lightPower = inputNumber ("lightPower: ");
            int airConditionerPower = inputNumber ("airConditionerPower: ");

            return new ControllerConfig (
                numberOfFloors,
                numberOfMainCorridorsPerFloor,
                numberOfSubCorridorsPerFloor,
                numberOfLightsPerMainCorridor,
                numberOfLightsPerSubCorridor,
                numberOfAirConditionersPerMainCorridor,
                numberOfAirConditionersPerSubCorridor,
                lightPower,
                airConditionerPower);
        }

        private static Action<string> log = (string line) => {
            Console.WriteLine (line);
        };

        private static Func<string, string> input = (message) => {
            Console.Write (message);
            return Console.ReadLine ();
        };

        private static Func<string, int> inputNumber = (message) => {
            Console.Write (message);
            string number = Console.ReadLine ();
            return int.Parse (number, System.Globalization.NumberStyles.Number);
        };
    }
}