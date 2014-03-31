//KOS: Call home to the archive and obtain the stats
// about a named parachute.
// ASSUMPTION: YOU WANT TO END UP ON VOLUME 1 WHEN
// DONE.
//
declare parameter chName.
// Change this one next line when querying volume name in a future release:
set prevVol to "1".
print "Contacting mission control to get stats.".
switch to archive.
run chuteDB(chName).
print "Returning to local volume: " + prevVol.
switch to 1. // Use local drive to store self-modifying code.
log "dummy" to tmpCmd.
delete tmpCmd.
set cmd to "switch t" + "o " + prevVol + ".".
log cmd to tmpCmd.
run tmpCmd.