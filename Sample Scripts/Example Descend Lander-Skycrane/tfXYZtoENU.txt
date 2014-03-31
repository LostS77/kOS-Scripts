// Given an XYZ coord in the KSP native coord system,
// calculate the same coord in terms of the ENU
// system (ENU is a term I made up for:
// "East North Up".  It's the system with an origin
// point on the surface of the SOI body directly 
// beneath the vessel, and with X=east, Y=north,
// and Z=up.
//
// Because you can't pass things into or out of
// a program, global variables must be used here
// to simulate that.
// INPUT:  x,y,z,e,n,u
// OUTPUT: tfE,tfN,tfU as global variables.
//   (As of KOS 0.65 there is no way to return a
//   value or pass a variable by reference so globals
//   have to be used for the return values.)
//
// All "local" variables begin with "tf" to help
// prevent them from clashing with the other
// variables you might have used in the global
// namespace of KOS.

declare parameter tfX,tfY,tfZ.

// Rotation angles for rotation matrix:
set tfA to latitude. 
set tfCosA to cos(tfA).
set tfSinA to sin(tfA).

set tfB to (0 - up:yaw).  // use UP:yaw like it was longitude
set tfCosB to cos(tfB).
set tfSinB to sin(tfB).

// The rotation matrix around z axis (latitude) then y axis (longitude):
set tfW to tfX*tfCosB            + 0          + tfZ*tfSinB            .
set tfN to tfX*tfSinA*tfSinB     + tfY*tfCosA + tfZ*(0-tfSinA*tfCosB) .
set tfU to tfX*(0-tfCosA*tfSinB) + tfY*tfSinA + tfZ*tfCosA*tfCosB     .

// Native XYZ is left-handed-system.  BUT ENU matches the
// Lat/Lon system which is right-handed. So the above rotation
// is calculated for a westerly axis then flipped to east here:
set tfE to 0 - tfW.