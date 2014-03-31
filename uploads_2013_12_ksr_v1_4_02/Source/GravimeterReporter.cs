using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSP;
using UnityEngine;

namespace kOSSensorReporter
{
    public class GravimeterReporter : SensorReporter  
    {
        public GravimeterReporter()
            : base("grav")
        {
        }

        public override double GetValue()
        {
            ModuleEnviroSensor sensor = (ModuleEnviroSensor)part.Modules["ModuleEnviroSensor"];
            if (sensor.sensorActive)
                return FlightGlobals.getGeeForceAtPosition(this.transform.position).magnitude;
            else
                return double.NaN;
        }
    }
}
