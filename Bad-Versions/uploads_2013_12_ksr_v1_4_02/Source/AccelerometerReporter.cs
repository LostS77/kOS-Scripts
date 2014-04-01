using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP;

namespace kOSSensorReporter
{
    public class AccelerometerReporter : SensorReporter 
    {
        public AccelerometerReporter()
            : base("accel")
        {
        }

        public override double GetValue()
        {
            ModuleEnviroSensor sensor = (ModuleEnviroSensor)part.Modules["ModuleEnviroSensor"];
            if (sensor.sensorActive)
                return vessel.geeForce_immediate;
            else
                return double.NaN;
        }
    }
}
