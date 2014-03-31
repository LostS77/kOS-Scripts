//KOS
declare parameter descType. // "hover", "lander", or "skycrane", or "skycrane/lander".
declare parameter seekFlat. // Flatness slope level acceptable for landing.
declare parameter numCh.    // Number of parachute parts on lander.
declare parameter chName.   // Name of the parachute parts on lander.

// Some sanity checks to stop the program if the user is trying to
// use it wrong:
//
sanityOK on.
if maxthrust = 0 { 
  print "ABORTING 'descend' PROGRAM: ".
  print "  No active engines right now.".
  sanityOK off.
}.  
if alt:periapsis > descendTop {
  print "ABORTING 'descend' PROGRAM: ".
  print "  Get your periapsis below " + descendTop + "m before running this program".
  sanityOK off.
}.
set chMass to 0.
if numCh > 0 {
  run chutedata(chName).
  if chMass = 0 {
    sanityOK off.
  }.
}.
// This would be cleaner if KOSscript had a premature
// exit or quit or return statement of some sort, rather
// than wrapping the majority of the code in one long IF
// body:
//
if sanityOK {
  print "Descend mode : " + descType .

  // Smallest payload mass the script will handle.
  // If the loss of mass between iterations is less
  // than this, it assumes that's just from
  // spending fuel.  If it's bigger, it assumes 
  // the payload has been dropped:
  set minPayload to 0.1 .

  print "Descend program initialised. ".
  print "---------------------------- ".
  print " ".
  print "Program will start operations when ".
  print "your AGL is under " + descendTop + " meters.".
  wait until ( altitude < descendTop ) .

  print "Taking over rotation control.".

  SAS OFF.
  // Steer orbital retrograde for now.  Will use surface further
  // down.  This is a quick starter to get KOS's rotation control
  // mostly in the right direction before the thrusters kick in.
  set mySteer to retrograde.
  lock steering to mySteer.

  // Difference between my desired orientation and my actual
  // orientation.  As this approaches zero I'm closer to being
  // aligned to my desired steering:
  // The reason for all the ugly trig is that this is a way to recognize
  // that, for example, 1 degree and 359 degrees are really only 2 degrees
  // apart, not 358 degrees apart).
  lock align to abs( cos(facing:pitch) - cos(mySteer:pitch) )
                + abs( sin(facing:pitch) - sin(mySteer:pitch) )
		+ abs( cos(facing:yaw) - cos(mySteer:yaw) )
		+ abs( sin(facing:yaw) - sin(mySteer:yaw) ) .
		// Roll had to be taken out because KOS doesn't align to it:
		// + abs( cos(facing:roll) - cos(mySteer:roll) )
		// + abs( sin(facing:roll) - sin(mySteer:roll) ).

  wait until align < 0.2.

  set myTh to 0.0 .
  lock throttle to myTh.

  print "ENTERING DESCENT MODE.".

  // dModes are as follows:
  // dMode 0 = descending to descendBot AGL.
  // dMode 1 = hovering at descendBot AGL.
  // dMode 2 = skycrane drop payload and escape.
  // dMode 3 = coming down for final touchdown from descendBot.
  set dMode to 0.

  set beDone to 0.

  set chutesYet to 0.

  // variables for slope seeking:
  set slope to 0.  // Slope detected under lander.
  set gHeight to 0.  // Height of ground above sea level.
  set pgHeight to 0.  // previous Height of ground above sea.
  set pTime to missiontime. // Previous elapsed time.

  set needNewAG9 to 1.

  clearscreen.
  print "*== MODE: ==========================*".
  print "|     AGL Altitude =                |".
  print "|    Periapsis alt =                |".
  print "| Term. vel at Pe  =                |".
  print "|   Current Thrust =                |".
  print "|    Waiting for areobrake ? =      |".
  print "|   Neutral Thrust =                |".
  print "|              TWR =                |".
  print "|    Current Speed =                |".
  print "|  Preferred Speed =                *----------*".
  print "| Ground Slope Here=                           |".
  print "| Duration of prev iteration =                 |".
  print "*----------------------------------------------*".
  print " ========= Descend Type: " + descType + " ====". 

  // Some lock expressions I will be using to cut down on
  // number of statements in the loop:
  // ====================================================
  lock heregrav to gConst*bodyMass/((altitude+bodyRadius)^2).
  lock twr to maxthrust/(heregrav*mass).
  set tfE to 9999999. set tfN to 9999999. set tfU to 9999999. // east,north,up vector
  lock absspd to (tfE^2 + tfN^2 + tfU^2) ^ 0.5 .
  set petermv to 999999.
  set usepe to 999999.

  // How much of my current aiming direction is vertical?
  // (i.e. cosine of angle between steering and straight up):
  set absvsup to abs(tfU).
  lock cossteerup to absvsup / ( (tfE^2+tfN^2+absvsup^2)^0.5 ).
  lock sinsteerup to ((tfE^2+tfN^2)^0.5) / ( (tfE^2+tfN^2+absvsup^2)^0.5 ).

  // If there's no atmosphere now, or if there is but later on
  // we slow down below terminal velocity, stop calcluating
  // the atmo drag as it tends to be an expensive calculation:
  loopedOnce off.
  when bodyAtmSea = 0 or (loopedOnce and (absspd < petermv) and usepe <= bodyMaxElev ) then {
    unlock fdrag. set fdrag to 0.
    unlock usepe. set usepe to 0.
    unlock pegrav. set pegrav to gConst*bodyMass/(bodyRadius^2).
    unlock pepress. set pepres to 0.
    unlock vdrag. set vdrag to 0.2 .
    unlock petermv. set petermv to 999999.
  }.
  // A lot of this stuff is only useful if there's air and
  // therefore drag needs to be taken into account.  It's a lot
  // of time consuming calculation so it should be eliminated
  // when there's no air, to speed up the loop:
  // ----------------------------------------------------------
  if bodyAtmSea > 0 {
    // Which periapsis to use: real or ground level?
    lock usepe to alt:periapsis. 
    when usepe < bodyMaxElev then {
      lock usepe to bodyMaxElev.
    }.
    // gravitational accelleration and pressure at periapsis:
    lock pegrav to gConst*bodyMass/((usepe+bodyRadius)^2).
    lock pepress to (atmToPres*bodyAtmSea) * e ^ ( (0-usepe)/bodyAtmScale) .
    // The current drag to calculate with:
    set useDrag to 0.2 .
    // The drag number to use will change when we hit certain conditions:
    if numCh > 0 {
      when pepress > chSemiPres then { set useDrag to chSemiDrag. }.
      when alt:periapsis < chFullAGL then { set useDrag to chFullDrag. }.
    }.
    lock vdrag to ( (mass-(numCh*chMass))*0.2 + (chMass*useDrag) ) / mass .
    // Force of drag from air pressure here:
    // current velocity when we got there:
    lock fdrag to 0.5*( (atmToPres*bodyAtmSea) * e^( (0-altitude)/bodyAtmScale) )*absspd^2*vdrag*0.008*mass.
    // Terminal velocity at periapsis:
    lock petermv to ( (250*pegrav)/( pepress * vdrag ) ) ^ 0.5 .
  }.

  // Hover throttle setting that would make my rate of descent constant:
  lock hovth to ((mass*heregrav)-fdrag) * (1/cossteerup) / maxthrust .

  // The acceleration I can do above and beyond what is needed to hover:
  lock extraac to (twr - 1) * heregrav.

  // My current stopping distance I need at that accelleration, with a 1.2x fudge
  // factor for a safety margin:
  lock stopdist to 1.2 * ( (absvsup-descendBotSpeed)^2)/(2*extraac).

  // naptime = Seconds to slow down the loop execution by.
  // Fast execution speed is only needed when near the bottom of the descent.
  // At the top it's safe to execute slowly and give more time to other KSP things:
  lock naptime to 10 * (alt:radar - descendBot)/(descendTop-descendBot).

  run tfXYZtoENU( velocity:surface:x, velocity:surface:y, velocity:surface:z ).
  set surfGrav to gConst*bodyMass/(bodyRadius^2).
  set surfExtraAc to ( (maxthrust/(mass*surfGrav) ) - 1 ) * surfGrav .

  until beDone {
    set airbrakeMult to 1. // 1 = not areobraking, 0 = areobraking.

    // Get surface velocity in terms of a coordinate system
    // based on the current East,North, and Up axes:
    // ...................................................
    run tfXYZtoENU( velocity:surface:x, velocity:surface:y, velocity:surface:z ).
    set absvsup to abs(tfU).

    // Try to save a snapshot of the dynamic data at one 
    // instant of time or as close as possible to that:
    // ...................................................
    // set absspd to (tfE^2 + tfN^2 + tfU^2) ^ 0.5 .
    set altAGL to alt:radar .
    set altSea to altitude .
    set spdSurf to surfacespeed.
    set dTime to missiontime - pTime.
    set pTime to missiontime.

    if bodyAtmSea > 0 {
      print "                          " at (22,2).
      if pepress > 0 and petermv < 10000 {
	print round( usepe* 10 ) / 10 + " m"  at (22,2).
      }.
      if usepe = bodyMaxElev {
	print "(est max elev)"  at (22,12).
      }.
      print "              " at (22,3).
      if pepress > 0 and petermv < 10000 {
	print round( petermv* 10 ) / 10  + " m/s" at (22,3).
      }.
    }.

    if dMode = 0 { print "  DESCENDING   " at (10,0). }.
    if dMode = 1 { print "   HOVERING    " at (10,0). }.
    if dMode = 2 { print "DEPLOYED/ESCAPE" at (10,0). }.
    if dMode = 3 { print "    LANDING    " at (10,0). }.

    print "        " at (32,11).
    print ( round( dTime * 100 ) / 100 ) + " s" at (32,11).

    // Guess AGL if we're too high to tell:
    if altAGL = altSea and altSea > 10000 {
      set altAGL to altSea - (bodyMaxElev/2).
    }.
    print "              " at (22,1).
    print ( round( altAGL * 10 ) / 10 ) + " m" at (22,1).


    set gHeight to (altSea-altAGL).
    set slope to 0.
    // Avoid calculating ground slope if not moving horizontally
    // fast enough to get a reliable reading.  If moving nearly
    // vertically, then the arithmetic gets chaotic and swingy:
    if spdSurf > 0.1 {
      set slope to (gHeight-pgHeight)/(dTime*spdSurf).
    }
    set pgHeight to gHeight.
    print "                        " at (22,10).
    print ( round( slope * 100 ) / 100 )  at (22,10).
    if abs(slope) > abs(seekFlat) {
      print "(seeking flatter)" at (28,10).
    }.
    print "            " at (22,7).
    print ( round( twr * 100) / 100 ) + " (at here)" at (22,7).
    print "            " at (22,8).
    print ( round( absspd * 100 ) / 100 ) + " m/s" at (22,8).

    if altAGL < descendBot*3 {
      // Check to see if we need to scope on further for
      // flatter land.  Only do this if landing has not
      // alrady begun.  Once it's begun commit to it:
      if dMode < 2 and abs(slope) > abs(seekFlat) {
	// Pretend the centered direction is a bit off from
	// what it really is, to make the code steer a bit
	// off on purpose, to make it seek a different
	// landing spot.
	if tfE > 0  { set tfE to tfE-10 . }.
	if tfE < 0  { set tfE to tfE+10 . }.
	if tfN > 0 { set tfN to tfN-10 . }.
	if tfN < 0 { set tfN to tfN+10 . }.
      }.
      // When near the bottom of the descent,
      // and the vertical speed gets slow, never
      // allow the sideways-pointing nature
      // of the velocity vector to cause the vessel
      // to steer too far off from upward.  Put an
      // upper limit on how much its allowed to tilt
      // sideways:
      set oldAbsVsUp to absvsup.
      if dMode = 1 or dMode = 3 { set absvsup to absspd + 1.0 . }.
    }.
    set mySteer to up * V( tfE, 0 - tfN, absvsup ).
    
    print "          " at (22,6).
    print ( round( hovth * 1000 ) / 10 ) + " %"  at (22,6).

    if dMode = 0 and altAGL > 0 and altAGL < descendBot {
      // Regardless of mode, go to hover mode first, and
      // stay there until slope is acceptable before
      // going to land or skycrane mode.
      set dMode to 1.
      BRAKES ON.
      LEGS ON.
      LIGHTS ON.
    }.
    if dMode = 1 {
      set pDesSpd to 5 * ( altAGL - descendBot ) / descendBot  .
      if needNewAG9 = 1 {
	on AG9 set dMode to 3. 
	on AG9 set needNewAG9 to 1.
	set needNewAG9 to 0.
      }.
      // If we really aren't supposed to hover but instead
      // are supposed to land or deploy, then only continue
      // to hover as long as slope is unacceptable for landing:
      if abs(slope) < abs(seekFlat) {
	if descType = "skycrane" or descType = "skycrane/lander" or descType = "lander" {
	  set dMode to 3.
	}.
      }.
    }.
    if dMode = 0 and altAGL > descendBot {

      // desired speed is based on estimate of the
      // suicide burn stopping distance, plus a bit of fudge
      // factor to handle not being able to see how high the terrain
      // is downrange from here where the landing will be:
      // (i.e. I might CURRENTLY be 2000 meters above the ground HERE,
      // but only 500 meters above the ground up ahead where the landing
      // will be.  The likelyhood of this problem goes down the more
      // and more vertical I am going, so use cossteerup to minimize
      // the magnitude of this extra guess the closer to vertical I am:
      set guessMoreH to (bodyMaxElev-gHeight).
      if guessMoreH > altAGL { set guessMoreH to 0. }.
      // Height to use to calculate how much distance I have available to stop:
      set H to (altAGL-descendBot) - (guessMoreH*sinsteerup). 
      // The correct formula is
      //                 ______
      //    pDesSpd =  \/ 2aH
      // But I changed the 2 to 1.8 to compensate for the delay of KOS
      // code always being a bit behind the curve.
      if H < 0  { set H to 0.  } // if dipping negative, don't allow sqrt to give NAN result.
      set pDesSpd to sqrt( 1.8 * surfExtraAc * H ) + descendBotSpeed.

      // Special case: If currently on a path that will bypass the ground (i.e.
      // periapsis wasn't set low enough when the program was started) then
      // force a burn until the periapsis is low enough:
      if periapsis > 0 and petermv > (absspd*1.3) {
        set pDesSpd to 0.
      }.

      // The decision whether or not to rely on areobraking
      // is complex.  For it to be a good idea not to use the
      // engines yet the following must be true:
      // - There is air at periopsis altitude.
      // - I am going a good deal faster than whatever the
      //     terminal velocity at periapsis is.
      // - My distance to the bottom is long enough to allow me
      //     to stop if I had to rely entirely on engines at my
      //     current TWR (this check is what causes engines to
      //     turn on when low to the ground but still going faster
      //     than terminal velocity).
      if  bodyAtmSea > 0 and absspd > petermv*1.25 and stopdist < (altAGL-descendBot)/cossteerup {
	set airbrakeMult to 0.
	if numCh > 0 and chutesYet = 0 {
	  CHUTES ON.
	  set chutesYet to 1.
	}.
      }.
      if airbrakeMult = 1 { print "no " at (32,5).  }.
      if airbrakeMult = 0 { print "yes" at (32,5).  }.
    }.
    if dMode = 2 {
      // Make darn sure the craft isn't still trying to
      // compensate for horizontal motion when it
      // drops the payload.  It MUST be vertical and
      // not lifting.
      set mySteer to up.
      set myTh to hovth*2/3.
      SAS ON. // So the SAS ON will be inherited by the payload before dropping it.
      // Because "stage" sometimes doesn't do anything the first time,
      // This tries until it noticibly changes the vessel mass to prove
      // it really did break a piece off:
      set oldMass to 0.
      set n to 0.
      until n > 8 or mass < (oldmass-minPayload) {
	set oldMass to mass.
	print "Trying to drop stage.".
	wait 0.5 .
	STAGE.
	set n to n + 1.
      }.
      if n > 8 { print "I can't seem to stage a payload.  Giving up.".  }.
      SAS OFF.

      // Clear the payload drop zone straight up, and gently
      // so as not to tip the payload with my exhaust:
      set myTh to 1.2 * hovth . 
      if myTh > 1 { set myTh to 1. }.
      set mySteer to up.
      wait 2.

      // Thrust away for a bit, either to escape the payload drop
      // area or to gently to go a short distance away and land:
      if descType = "skycrane" {
	set mySteer to up + R(30,0,0).
	set myTh to 1.5*hovth.
	if myTh > 1 { set myTh to 1. }.
	set beDone to 1.
	wait 5.
      }.
      if descType = "skycrane/lander" {
	set mySteer to up + R(20,0,0).
	set myTh to hovth .
	if myTh > 1 { set myTh to 1. }.
	// Become a lander now:
	set descType to "lander".
	set dMode to 3.
	wait 3 .
      }.
    }.
    if dMode = 3 {
      set pDesSpd to bodyLandingSpeed.
      if STATUS = "LANDED" or STATUS = "SPLASHED" {
	if descType = "hover" or descType = "lander" {
	  // Stop moving:
	  set mySteer to up.
	  lock throttle to 0.
	  wait 10.
	  set beDone to 1. 
	}
	if descType = "skycrane" or descType = "skycrane/lander" {
	  SAS ON. // To ensure the payload is holding itself stable.
	  set dMode to 2.
	}.
      }.
    }.

    print "             " at (22,9).
    print ( round( pDesSpd * 100 ) /  100 ) + " m/s" at (22,9).
    set spd to absspd.
    if verticalspeed > 0.0 { set spd to 0 - absspd. }.

    // How far to offset the throttle depends on relatively how far
    // off we are from the desired speed, and how good the craft is at
    // thrusting, and how slowly this loop is running.
    // The goal being seeked is to use a setting that would achive the
    // desired speed in 1 iteration:
    set thOff to ( ( spd - pDesSpd ) / dTime ) / (maxthrust/mass).

    // Make the throttle less gentle when too close to the ground and going down
    // fast - go ahead and throttle highly if in that scenario:
    if altAGL < (descendBot*3) and spd > (pDesSpd*2) {
      set thOff to 1.5*thOff.
    }.
    
    set newTh to ( hovth + thOff ) * airbrakeMult .
    if newTh < 0.0 { set newTh to 0.0 . }.
    if newTh > 1.0 { set newTh to 1.0 . }.

    // If not even remotely close to the desired direction, then reduce
    // throttle, but not all the way to zero just in case the craft
    // depends on vectored thrust to help steer it:
    if align > 1.0 {
      set newTh to newTh / 3.
    }.
    set myTh to newTh.

    print "              " at (22,4).
    print ( round( myTh * 1000 ) / 10 ) + " %"  at (22,4).

    // extra delay when up high and not thrusting right now:
    if naptime > 0 and myTh = 0 { wait naptime. }.
    
    loopedOnce on.
  }.

}.

print "DONE.".