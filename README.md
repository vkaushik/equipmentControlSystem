# EquipmentControlSystem

###Steps to run:

1. Install .Net Core 3.0
   Refer: https://dotnet.microsoft.com/download/dotnet-core/3.0

2. Run following commands in terminal (Assuming you've working git command):
   1. git clone https://github.com/vkaushik/equipmentControlSystem.git
   2. cd equipmentControlSystem/controllerTest
   3. dotnet test

---

```
Algorithm to handle event:

    On signal event
        if(eventType == movement)
            allowedPowerConsumption
            futurePowerConsumption
            extraPowerConsumption = allowedPowerConsumption - futurePowerConsumption

            if(extraPowerConsumption > 0)
                accomodateByDecreasingPower(extraPowerConsumption)
                switch on equipments

        else if (eventType == noMovement )
            presentPowerConsumption
            futurePowerConsumption
            changeInPowerConsumption = presentPowerConsumption - futurePowerConsumption

            if(changeInPowerConsumption > 0)
                switch off equipments
                accomodateByIncreasingPower(changeInPowerConsumption)

accomodate increased power consumption
    get switchedOnInactiveSubCorridorAirConditioners
    reducedPowerConsumption = 0
    foreach(ac in switchedOnInactiveSubCorridorAirConditioners)
        if (reducedPowerConsumption >= extraPowerConsumption) break
        ac.switchOff()
        reducedPowerConsumption += ac.power
        equipmentsSwitchedOffToAdjustPowerConsumption.add(ac)

accomodate decreased power consumption
    increasedPowerConsumption = 0
    foreach(equipment in equipmentsSwitchedOffToAdjustPowerConsumption)
        if (increasedPowerConsumption >= changeInPowerConsumption) break
        equipment.switchOn()
        increasedPowerConsumption += equipment.power;
        equipmentsSwitchedOffToAdjustPowerConsumption.remove(equipment)
```

Tasks:

- [x] Working code
- [ ] Edge cases handle in test
- [ ] Refactoring (for loops, interfaces)
