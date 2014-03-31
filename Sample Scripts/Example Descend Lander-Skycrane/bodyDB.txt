//KOS bodystats: sets up variables for use in programs
// that need stats about the orbital body that is the
// current SOI.  Looks up the body name and fills
// the appropriate variables.
declare parameter forBody.

print " ".
print "Home base Body Database successfully contacted.".
print " ".

// Constants for important things that DON'T change per body:
// ==========================================================
set gConst to 6.67384*10^(0-11). // The Gravitational constant
set pi to 3.14159265359 .
set e to 2.718281828459 .
set atmToPres to 1.2230948554874 . // presure at 1.0 Atm's.

// Starting defaults for constants that DO change per body:
// ========================================================
set bodySurfaceGrav to 10.   // The m/s^2 at the body's "sea level".
set bodyMass to 5.2915793*10^22. // Kilograms for body mass.
set bodyRadius to 500000 .   // The radius from center to equator.
set bodyMaxElev to 4000.     // The peak of the highest mountain.
set bodyAtmSea to 0.         // sea level atmosphere pressure in units of Kerbin atm's.
set bodyAtmScale to 0.       // atmospheric scale height.
set descendTop to 50000 .    // The highest AGL at which descents might start.
set descendBot to 100 .      // the AGL where a descending craft should hover.
set descendTopSpeed to 2000.0 . // Desired speed at top of descent profile.
set descendBotSpeed to 4.0 .    // Desried speed at bottom of desecnt profile.
set bodyLandingSpeed to 4.0.    // Desried speed to come down from hover to.

// Overrides for each body:
// ==========================

if forBody = "Kerbin" {
  set bodySurfaceGrav to 9.802 .
  set bodyRadius to 600000 .
  set bodyMass to 5.2915793*10^22. 
  set bodyMaxElev to 6761 .
  set bodyScaleHeight to 5000 . 
  set bodyAtmSea to 1.0 .
  set bodyAtmScale to 5000 .
  set descendTop to 70000 .
  set descendBot to 150.
  set descendTopSpeed to 2100.0 .
  set descendBotSpeed to 6.0 .
  set descendLandingSpeed to 4.0 .
}.
if forBody = "Mun" {
  set bodySurfaceGrav to 1.63 .
  set bodyRadius to 200000 .
  set bodyMass to 9.7600236*10^20.
  set bodyMaxElev to 7061 .
  set descendTop to 20000 .
  set descendBot to 80 .
  set descendTopSpeed to 542.0 .
  set descendBotSpeed to 6.0 .
  set bodyLandingSpeed to 3.0 .
}.
if forBody = "Minmus" {
  set bodySurfaceGrav to 0.491 .
  set bodyRadius to 60000 .
  set bodyMass to 2.6457897*10^19.
  set bodyMaxElev to 5725 .
  set descendTop to 20000 .
  set descendBot to 50 .
  set descendTopSpeed to 274.0 .
  set descendBotSpeed to 5.0 .
  set bodyLandingSpeed to 2.0 .
}.
if forBody = "Duna" {
  set bodySurfaceGrav to 2.94 .
  set bodyRadius to 320000 .
  set bodyMass to 4.5154812*10^21.
  set bodyMaxElev to 8264.
  set bodyAtmSea to 0.2 .
  set bodyAtmScale to 3000.
  set descendTop to 40000 .
  set descendBot to 100 .
  set descendTopSpeed to 1000.0 .
  set descendBotSpeed to 5.0 .
  set bodyLandingSpeed to 3.0 .
}.
if forBody = "Ike" {
  set bodySurfaceGrav to 1.10 .
  set bodyRadius to 130000 .
  set bodyMass to 2.7821949*10^20.
  set bodyMaxElev to 12750 .
  set descendTop to 12000 .
  set descendBot to 50 .
  set descendTopSpeed to 534 .
  set descendBotSpeed to 5.0 .
  set bodyLandingSpeed to 3.0 .
}.

print "======== " + forBody + " ===================".
print "bodySurfaceGrav  = " + bodySurfaceGrav.
print "bodyRadius       = " + bodyRadius.
print "bodyMass         = " + bodyMass.
print "bodyAtmSea       = " + bodyAtmSea.
print "bodyAtmScale     = " + bodyAtmScale.
print "descendTop       = " + descendTop.
print "descendBot       = " + descendBot.
print "descendTopSpeed  = " + descendTopSpeed.
print "descendBotSpeed  = " + descendBotSpeed.
print "bodyLandingSpeed = " + bodyLandingSpeed.
print "=============================================".
print " ".
print "'AGL' means Above Ground Level to distinguish ".
print "from sea level altitude.".
print " ".
print "You may change these variables with the 'set' ".
print "command before running other programs to try ".
print "other settings.".
print " ".