//KOS
declare parameter chName . // parachute name to look up, must be full name.
set chMass to 0.
set chSemiPres to 0.
set chSemiDrag to 0.
set chFullAGL to 0.
set chFullDrag to 0.

if chName = "Mk16 Parachute" {
  set chMass to 0.1 .
  set chSemiPres to 0.01 .
  set chSemiDrag to 1 .
  set chFullAGL to 500 .
  set chFullDrag to 500 .
}.
if chName = "Mk16-XL Parachute" {
  set chMass to 0.3 .
  set chSemiPres to 0.01 .
  set chSemiDrag to 1 .
  set chFullAGL to 500 .
  set chFullDrag to 500 .
}.
if chName = "Mk2-R Radial-Mount Parachute" {
  set chMass to 0.15 .
  set chSemiPres to 0.01 .
  set chSemiDrag to 1 .
  set chFullAGL to 500 .
  set chFullDrag to 500 .
}.
if chName = "Mk25 Parachute" {
  set chMass to 0.2 .
  set chSemiPres to 0.007 .
  set chSemiDrag to 4 .
  set chFullAGL to 2500 .
  set chFullDrag to 170 .
}.
if chMass = 0 {
  print " ".
  print "DB lookup error: No data for parachute named: ".
  print "    '" + chName + "'.".
  print " ".
}.