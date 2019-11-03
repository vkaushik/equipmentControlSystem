using System;

namespace EquipmentControlSystem.Controller {
    public class EquipmentControllerFactory {
        public static IEquipmentController CreateControllerUsing (ControllerConfig config, Action<string> log) {
            var equipmentController = new EquipmentController (config, log);
            SetUp (equipmentController, config);
            equipmentController.ActivateNightSlot ();
            return equipmentController;
        }

        private static void SetUp (IEquipmentController equipmentController, ControllerConfig config) {
            for (int floorId = 1; floorId <= config.numberOfFloors; ++floorId) {
                for (int mainCorridorId = 1; mainCorridorId <= config.numberOfMainCorridorsPerFloor; ++mainCorridorId) {
                    for (short sequenceNumber = 1; sequenceNumber <= config.numberOfLightsPerMainCorridor;
                        ++sequenceNumber) {
                        // Create and install MainCorridor Lights
                        var equipmentId = new EquipmentId (floorId.ToString (), $"MC-{mainCorridorId}",
                            EquipmentType.Light, sequenceNumber.ToString ());
                        var mainCorridorLight = new Equipment (equipmentId, config.lightPower);

                        equipmentController.InstallMainCorridorEquipment (mainCorridorLight);
                    }
                    for (short sequenceNumber = 1; sequenceNumber <= config.numberOfAirConditionersPerMainCorridor;
                        ++sequenceNumber) {
                        // Create and install MainCorridor AirConditioners
                        var equipmentId = new EquipmentId (floorId.ToString (), $"MC-{mainCorridorId}",
                            EquipmentType.AirConditioner, sequenceNumber.ToString ());
                        var mainCorridorAirConditioner = new Equipment (equipmentId, config.airConditionerPower);

                        equipmentController.InstallMainCorridorEquipment (mainCorridorAirConditioner);
                    }
                }
                for (int subCorridorId = 1; subCorridorId <= config.numberOfSubCorridorsPerFloor; ++subCorridorId) {
                    for (short sequenceNumber = 1; sequenceNumber <= config.numberOfLightsPerSubCorridor;
                        ++sequenceNumber) {
                        // Create and install SubCorridor Lights with sensors
                        var equipmentId = new EquipmentId (floorId.ToString (), $"SC-{subCorridorId}",
                            EquipmentType.Light, sequenceNumber.ToString ());
                        var subCorridorLight = new Equipment (equipmentId, config.lightPower);

                        var sensorId = new EquipmentId (floorId.ToString (), $"SC-{subCorridorId}",
                            EquipmentType.Sensor, sequenceNumber.ToString ());
                        var sensor = new Sensor (sensorId);

                        equipmentController.InstallSubCorridorEquipmentWithSensor (subCorridorLight, sensor);

                    }
                    for (short sequenceNumber = 1; sequenceNumber <= config.numberOfAirConditionersPerSubCorridor;
                        ++sequenceNumber) {
                        // Create and install SubCorridor AirConditioners
                        var equipmentId = new EquipmentId (floorId.ToString (), $"SC-{subCorridorId}",
                            EquipmentType.AirConditioner, sequenceNumber.ToString ());
                        var subCorridorAirConditioner = new Equipment (equipmentId, config.airConditionerPower);

                        equipmentController.InstallSubCorridorEquipment (subCorridorAirConditioner);

                    }
                }
            }
        }
    }
}