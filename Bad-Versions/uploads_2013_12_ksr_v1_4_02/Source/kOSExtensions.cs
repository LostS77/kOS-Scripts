using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSP;

namespace kOSSensorReporter
{
    public class kOSExtensions : PartModule 
    {
        public override void OnStart(StartState state)
        {
            print("Adding math!pi");
            vessel.parts.ForEach(part => part.SendMessage("RegisterkOSExternalFunction", new object[] { "math!pi", this, "GetPi", 0 }));
            print("Adding math!log");
            vessel.parts.ForEach(part => part.SendMessage("RegisterkOSExternalFunction", new object[] { "math!log", this, "Log", 1 }));
            print("Adding math!logbase");
            vessel.parts.ForEach(part => part.SendMessage("RegisterkOSExternalFunction", new object[] { "math!logb", this, "LogBase", 2 }));
            print("Adding math!ln");
            vessel.parts.ForEach(part => part.SendMessage("RegisterkOSExternalFunction", new object[] { "math!ln", this, "Ln", 1 }));
            print("Adding math!e");
            vessel.parts.ForEach(part => part.SendMessage("RegisterkOSExternalFunction", new object[] { "math!e", this, "GetE", 0 }));
            print("Adding vessel!velocity");
            vessel.parts.ForEach(part => part.SendMessage("RegisterkOSExternalFunction", new object[] { "vessel!velocity", this, "GetVelocity", 0 }));
            print("Adding vessel!gravconst");
            vessel.parts.ForEach(part => part.SendMessage("RegisterkOSExternalFunction", new object[] { "physics!gravconst", this, "GetGravitationalConstant", 0 }));
            print("Adding vessel!isp");
            vessel.parts.ForEach(part => part.SendMessage("RegisterkOSExternalFunction", new object[] { "vessel!isp", this, "GetIsp", 0 }));
            base.OnStart(state);
        }

        public double GetPi()
        {
            return Math.PI;
        }

        /// <summary>
        /// Gets the natural number e.
        /// </summary>
        /// <returns></returns>
        public double GetE()
        {
            return Math.E;
        }

        /// <summary>
        /// Gets the base-10 logarithm of a number.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double Log(double x)
        {
            return Math.Log10(x);
        }

        /// <summary>
        /// Gets the given base logarithm of a given number.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public double LogBase(double x, double b)
        {
            return Math.Log(x, b);
        }

        /// <summary>
        /// Gets the natural logarithm (base e) of a given number.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double Ln(double x)
        {
            return Math.Log(x);
        }

        public double GetVelocity()
        {
            return vessel.obt_velocity.magnitude;
        }

        public double GetGravitationalConstant()
        {   
            return 6.67384E-11;
        }

        public double GetIsp()
        {
            double isp = 0;
            foreach (Part p in vessel.parts)
            {
                foreach (PartModule m in p.Modules)
                {
                    if (m is ModuleEngines)
                    {
                        if (((ModuleEngines)m).EngineIgnited) isp += ((ModuleEngines)m).realIsp;
                    }
                }
            }
            return isp;
        }
    }
}
