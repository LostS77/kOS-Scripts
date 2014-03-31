using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSP;
using UnityEngine;

namespace kOSSensorReporter
{
    public class BarometerReporter : SensorReporter
    {
        public BarometerReporter()
            : base("press")
        {
        }

        public override double GetValue()
        {
            ModuleEnviroSensor sensor = (ModuleEnviroSensor)part.Modules["ModuleEnviroSensor"];
            if (sensor.sensorActive)
                return part.staticPressureAtm;
            else
                return double.NaN;
        }
    }
}
