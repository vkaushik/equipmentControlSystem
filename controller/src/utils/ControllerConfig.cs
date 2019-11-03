using System;

namespace EquipmentControlSystem.Controller {
    public class ControllerConfig {
        public int numberOfFloors { get; set; }
        public int numberOfMainCorridorsPerFloor { get; set; }
        public int numberOfSubCorridorsPerFloor { get; set; }
        public int numberOfLightsPerMainCorridor { get; set; }
        public int numberOfLightsPerSubCorridor { get; set; }
        public int numberOfAirConditionersPerMainCorridor { get; set; }
        public int numberOfAirConditionersPerSubCorridor { get; set; }
        public int lightPower { get; set; }
        public int airConditionerPower { get; set; }

        public ControllerConfig () { }

        public ControllerConfig (
            int numberOfFloors,
            int numberOfMainCorridorsPerFloor,
            int numberOfSubCorridorsPerFloor,
            int numberOfLightsPerMainCorridor,
            int numberOfLightsPerSubCorridor,
            int numberOfAirConditionersPerMainCorridor,
            int numberOfAirConditionersPerSubCorridor,
            int lightPower,
            int airConditionerPower) {
            this.numberOfFloors = numberOfFloors;
            this.numberOfMainCorridorsPerFloor = numberOfMainCorridorsPerFloor;
            this.numberOfSubCorridorsPerFloor = numberOfSubCorridorsPerFloor;
            this.numberOfLightsPerMainCorridor = numberOfLightsPerMainCorridor;
            this.numberOfLightsPerSubCorridor = numberOfLightsPerSubCorridor;
            this.numberOfAirConditionersPerMainCorridor = numberOfAirConditionersPerMainCorridor;
            this.numberOfAirConditionersPerSubCorridor = numberOfAirConditionersPerSubCorridor;
            this.lightPower = lightPower;
            this.airConditionerPower = airConditionerPower;
        }
    }
}