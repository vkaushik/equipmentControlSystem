using System;
using System.Collections.Generic;

namespace EquipmentControlSystem.Controller {
    public class EquipmentController : IEquipmentController {
        private readonly ControllerConfig _config;
        private Action<string> _log;
        private readonly EquipmentGroup _mainCorridorEquipments = new EquipmentGroup ();
        private readonly EquipmentGroup _subCorridorEquipments = new EquipmentGroup ();
        private HashSet<Sensor> _sensors = new HashSet<Sensor> ();
        private readonly Queue<Equipment> _equipmentsSwitchedOffToAdjustPowerConsumption =
            new Queue<Equipment> ();
        private readonly Dictionary<Sensor, EquipmentGroup> sensorToEquipments =
            new Dictionary<Sensor, EquipmentGroup> ();
        private readonly HashSet<string> _activeSubCorridorIds = new HashSet<string> ();
        private readonly object _equipmentStateChange = new object ();
        private IDisposable unsubscriber;
        public EquipmentController (ControllerConfig config, Action<string> log) {
            this._config = config;
            this._log = log;
        }

        public void OnNext (Signal signal) {
            lock (_equipmentStateChange) {
                if (signal.type == SingalType.movement) {
                    var allowedPowerConsumption = getAllowedPowerConsumption ();
                    var increasedPowerConsumption = getIncreasedPowerConsumption (signal);
                    var extraPowerConsumption = increasedPowerConsumption - allowedPowerConsumption;
                    if (extraPowerConsumption > 0) {
                        accomodateByDecreasingPower (extraPowerConsumption, signal);
                    }
                    switchOnEquipmentsWith (signal);
                } else if (signal.type == SingalType.noMovement) {
                    var changeInPowerConsumption = getChangeInPowerConsumptionWhenSwitchOffWith (signal);
                    switchOffEquipmentsWith (signal);
                    if (changeInPowerConsumption > 0) {
                        accomodateByIncreasingPower (changeInPowerConsumption, signal);
                    }
                }
            }
        }

        private void accomodateByDecreasingPower (float extraPowerConsumption, Signal signal) {
            _activeSubCorridorIds.Add (signal.source.id.corridorId);

            var inactiveSubCorridorAirConditioners = new List<Equipment> ();
            foreach (var equipment in _subCorridorEquipments) {
                if (equipment.id.type is EquipmentType.AirConditioner) {
                    if (!_activeSubCorridorIds.Contains (equipment.id.corridorId)) {
                        inactiveSubCorridorAirConditioners.Add (equipment);
                    }
                }
            }

            float reducedPowerConsumption = 0;
            foreach (var equipment in inactiveSubCorridorAirConditioners) {
                if (!equipment.isOn) continue;
                if (reducedPowerConsumption >= extraPowerConsumption) break;
                equipment.switchOff ();
                reducedPowerConsumption += equipment.power;
                _equipmentsSwitchedOffToAdjustPowerConsumption.Enqueue (equipment);
            }
            if (reducedPowerConsumption < extraPowerConsumption) {
                _log ("Warning: Could not balance power consumption ! No more equipments to switch off");
                _log ($"Power overshoot by: {extraPowerConsumption - reducedPowerConsumption}");
            }
        }

        private void accomodateByIncreasingPower (float changeInPowerConsumption, Signal signal) {
            _activeSubCorridorIds.Remove (signal.source.id.corridorId);
            float increasedPowerConsumption = 0;
            while (_equipmentsSwitchedOffToAdjustPowerConsumption.Count > 0) {
                if (increasedPowerConsumption >= changeInPowerConsumption) break;
                var equipment = _equipmentsSwitchedOffToAdjustPowerConsumption.Dequeue ();
                equipment.switchOn ();
                increasedPowerConsumption += equipment.power;
            }
        }

        public void InstallMainCorridorEquipment (Equipment equipment) {
            _mainCorridorEquipments.Add (equipment);
        }

        public void InstallSubCorridorEquipment (Equipment equipment) {
            _subCorridorEquipments.Add (equipment);
        }

        public void InstallSubCorridorEquipmentWithSensor (Equipment equipment, Sensor sensor) {
            _subCorridorEquipments.Add (equipment);
            _sensors.Add (sensor);
            if (!sensorToEquipments.ContainsKey (sensor)) {
                sensorToEquipments[sensor] = new EquipmentGroup ();
            }
            sensorToEquipments[sensor].Add (equipment);
            sensor.Subscribe (this);
        }

        public void ActivateNightSlot () {
            _mainCorridorEquipments.SwitchOnAll ();
            foreach (var equipment in _subCorridorEquipments) {
                if (equipment.id.type is EquipmentType.AirConditioner) {
                    equipment.switchOn ();
                }
            }
        }
        public Sensor GetSensorById (EquipmentId sensorId) {
            foreach (var sensor in _sensors) {
                if (sensor.id.value == sensorId.value) {
                    return sensor;
                }
            }
            return null;
        }

        private float getAllowedPowerConsumption () {
            // returns: number of power units for given SubCorridor
            // Maximum power consumption for a floor 
            //    = (numberOfSubCorridorsPerFloor * 10) + (numberOfMainCorridorsPerFloor * 15);
            // Maximum power consumption for a SubCorridor is 10
            return 10;
        }

        private float getIncreasedPowerConsumption (Signal signal) {
            // returns: number of power units for given SubCorridor
            float changeInPowerConsumptionWhenSwitchOn = 0;

            foreach (var equipment in _subCorridorEquipments) {
                if (equipment.id.corridorId == signal.source.id.corridorId) {
                    changeInPowerConsumptionWhenSwitchOn += equipment.power;
                }
            }
            return changeInPowerConsumptionWhenSwitchOn;
        }

        private float getFuturePowerConsumptionWhenSwitchOnWith (Signal signal) {
            // returns: number of power units for given SubCorridor
            float futurePowerConsumptionOfSubCorridor = 0;
            foreach (var equipment in _subCorridorEquipments) {
                if (equipment.id.corridorId == signal.source.id.corridorId) {
                    futurePowerConsumptionOfSubCorridor += equipment.power;
                }
            }

            return futurePowerConsumptionOfSubCorridor;
        }

        private float getChangeInPowerConsumptionWhenSwitchOffWith (Signal signal) {
            // returns: number of power units for given SubCorridor
            float changeInPowerConsumptionWhenLightsSwitchedOff = 0;
            foreach (var equipment in _subCorridorEquipments) {
                if (equipment.id.corridorId == signal.source.id.corridorId &&
                    equipment.id.type == EquipmentType.Light &&
                    equipment.isOn) {
                    changeInPowerConsumptionWhenLightsSwitchedOff += equipment.power;
                }
            }

            return changeInPowerConsumptionWhenLightsSwitchedOff;
        }

        private void switchOnEquipmentsWith (Signal signal) {
            foreach (var equipment in _subCorridorEquipments) {
                if (equipment.id.corridorId == signal.source.id.corridorId) {
                    equipment.switchOn ();
                }
            }
        }

        private void switchOffEquipmentsWith (Signal signal) {
            sensorToEquipments[signal.source].SwitchOffAll ();
        }

        public void LogStatus () {
            var floorIds = new HashSet<string> ();
            var floorIdToMainCorridorEquipmentStatus = new Dictionary<string, List<string>> ();
            foreach (var equipment in _mainCorridorEquipments) {
                var floorId = equipment.id.floorId;
                if (!floorIdToMainCorridorEquipmentStatus.ContainsKey (floorId)) {
                    floorIdToMainCorridorEquipmentStatus[floorId] = new List<string> ();
                    floorIds.Add (floorId);
                }
                var corridorId = equipment.id.corridorId.Split (EquipmentId.idSeparator) [1];
                var equipmentType = equipment.id.type.ToString ();
                var sequenceNumber = equipment.id.sequenceId;
                var equipmentStatus = equipment.isOn ? "ON" : "OFF";
                var statusLine = $"Main corridor {corridorId} {equipmentType} {sequenceNumber} : {equipmentStatus}";
                floorIdToMainCorridorEquipmentStatus[floorId].Add (statusLine);
            }

            var floorIdToSubCorridorEquipmentStatus = new Dictionary<string, List<string>> ();
            foreach (var equipment in _subCorridorEquipments) {
                var floorId = equipment.id.floorId;
                if (!floorIdToSubCorridorEquipmentStatus.ContainsKey (floorId)) {
                    floorIdToSubCorridorEquipmentStatus[floorId] = new List<string> ();
                    floorIds.Add (floorId);
                }
                var corridorId = equipment.id.corridorId.Split (EquipmentId.idSeparator) [1];
                var equipmentType = equipment.id.type.ToString ();
                var sequenceNumber = equipment.id.sequenceId;
                var equipmentStatus = equipment.isOn ? "ON" : "OFF";
                var statusLine = $"Sub  corridor {corridorId} {equipmentType} {sequenceNumber} : {equipmentStatus}";
                floorIdToSubCorridorEquipmentStatus[floorId].Add (statusLine);
            }

            foreach (var floorId in floorIds) {
                _log ($"Floor {floorId}");
                if (floorIdToMainCorridorEquipmentStatus.ContainsKey (floorId)) {
                    foreach (var status in floorIdToMainCorridorEquipmentStatus[floorId]) {
                        _log (status);
                    }
                }
                if (floorIdToSubCorridorEquipmentStatus.ContainsKey (floorId)) {
                    foreach (var status in floorIdToSubCorridorEquipmentStatus[floorId]) {
                        _log (status);
                    }
                }
                _log ("");
            }
        }

        public virtual void Subscribe (IObservable<Signal> provider) {
            unsubscriber = provider.Subscribe (this);
        }

        public virtual void Unsubscribe () {
            unsubscriber.Dispose ();
        }

        public void OnCompleted () {
            throw new NotImplementedException ();
        }

        public void OnError (Exception error) {
            throw new NotImplementedException ();
        }
    }
}