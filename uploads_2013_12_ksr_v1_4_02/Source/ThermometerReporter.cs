using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSP;
using UnityEngine;

namespace kOSSensorReporter
{
    public class ThermometerReporter : SensorReporter 
    {
        public ThermometerReporter()
            : base("temp")
        {
            
        }

        public override double GetValue()
        {
            ModuleEnviroSensor sensor = (ModuleEnviroSensor)part.Modules["ModuleEnviroSensor"];
            if (sensor.sensorActive)
                return part.temperature;
            else
                return double.NaN;
        }
    }
}
