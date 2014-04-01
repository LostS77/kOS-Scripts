using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace kOSSensorReporter
{
    public abstract class SensorReporter : PartModule 
    {
        string kosName;

        public SensorReporter(string name)
        {
            kosName = name;
        }

        public override void OnStart(StartState state)
        {
            if (state != StartState.Editor)
            {
                print("Adding sensor!" + kosName);
                vessel.parts.ForEach(part => part.SendMessage("RegisterkOSExternalFunction", new object[] { "sensor!" + kosName, this, "GetValue", 0 }));
                print("Adding sensor!" + kosName + "!str");
                vessel.parts.ForEach(part => part.SendMessage("RegisterkOSExternalFunction", new object[] { "sensor!" + kosName + "str", this, "GetReadout", 0 }));
                print("Adding sensor!" + kosName + "!toggle");
                vessel.parts.ForEach(part => part.SendMessage("RegisterkOSExternalFunction", new object[] { "sensor!" + kosName + "!toggle", this, "KOSToggle", 0 }));
                print("Adding sensor!" + kosName + "!active");
                vessel.parts.ForEach(part => part.SendMessage("RegisterkOSExternalFunction", new object[] { "sensor!" + kosName + "!active", this, "SensorActive", 0 }));
            }
            
            base.OnStart(state);
        }

        public abstract double GetValue();

        public string GetReadout()
        {
            ModuleEnviroSensor sensor = (ModuleEnviroSensor)part.Modules["ModuleEnviroSensor"];
            
            return sensor.readoutInfo;
        }

        public bool KOSToggle()
        {
            ModuleEnviroSensor sensor = (ModuleEnviroSensor)part.Modules["ModuleEnviroSensor"];
            return sensor.sensorActive = !sensor.sensorActive;
        }

        public bool SensorActive()
        {
            ModuleEnviroSensor sensor = (ModuleEnviroSensor)part.Modules["ModuleEnviroSensor"];
            return sensor.sensorActive;
        }
    }
}
