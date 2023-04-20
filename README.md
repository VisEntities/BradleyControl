# Bradley on steroids! From bland to badass


--------

## Configuration
```json
{
  "Version": "1.0.0",
  "Health": {
    "Starting Health": 1000.0,
    "Maximum Health": 1000.0
  },
  "Loot": {
    "Maximum Crates To Drop": 3
  },
  "Flame": {
    "Set Crates On Fire": true,
    "Minimum Life Time": 20.0,
    "Maximum Life Time": 40.0,
    "Spread Chance": 50,
    "Spread At Lifetime Percent": 50,
    "Damage Per Second": 2.0,
    "Damage Rate": 0.5,
    "Water Required To Extinguish": 200
  },
  "Debris": {
    "Drop On Destruction": true,
    "Harvestable Hit Points": 500.0,
    "Harvest Cooldown": 480.0
  },
  "Movement": {
    "Maximum Speed": 2000.0,
    "Spin Speed": 2000.0,
    "Brake Force": 100.0
  },
  "Targeting": {
    "Engagement Range": 100.0,
    "Target Search Range": 100.0,
    "Memory Duration": 20.0
  },
  "Coax Turret": {
    "Time Between Bursts": 0.06667,
    "Maximum Shots Per Burst": 10,
    "Bullet Damage": 15.0
  },
  "Cannon": {
    "Recoil Intensity": 200.0
  }
}
```

### Set Crates On Fire
Determines whether or not to create fire balls for each crate that spawns after Bradley's destruction

### Spread Chance
Determines the probability of a fireball spreading and creating new fireballs.
The higher value, the fireball is less likely to spread further, while lower value increases the chance of fireball spreading
Keep in mind that too many fireballs may negatively impact you server's performance,, so it's important to have the performance in mind and considered.

### Spread At Lifetime Percent
Determines at what time of the fireball lifetime to being spreading sub fireballs further.

------

### Maximum Speed
Determines the maximum torque applied to the wheels for forward and backward movement, which affects the maximum speed Bradley can reach.

### Spin Speed
Determines how fast or slow Bradley turns or spins.
The higher value, the faster spinning, making Bradley more agile and responsive. While lower value makes it less agile and harder to maneuver.

### Brake Force
Determines the strength of the vehicle's brakes, affecting Bradley's deceleration rate and stopping distance.
Higher values will result in a more contained fire, while lower values will lead to a fire that spreads more aggressively.

------------

### Engagement Range
Determines the maximum distance within which the player can be interacted or engaged with by Bradley. Players beyond this range of visibility are considered invalid for engagement and unreachable by Bradley's weapons.

### Target Search Range
Determines the radius around Bradley within which it can see and look for potential targets.
Higher value allows Bradley to track targets at a greater distance while lower value limits the detection and tracking range.

### Memory Duration
Determines how long Bradley remembers a target after it has last seen him. Basically makes Bradley lose track of targets that it hasn't seen or engaged with for a certain period.
Higher values makes Bradley remember target for a longer duration, even if they go out of sight, lower value makes it forget them ore quickly once they go out of sight

----------

### Time Between Bursts
Represents the time between each shot of the coaxial gun.
Higher value makes shoot less frequently, while lower value makes it short more frequently.

### Maximum Shots Per Burst
Represents the maximum number of shots fired in a single burst by the coaxial gun.
Higher value allows  more shots to be fired in a single burst, while lower fiew shots.

### Bullet Damage
Represents the base amount of damage dealt by the bullets fired by the coaxial gun.

-------

### Recoil Intensity
Determines the amount of force applied to Bradley's body as a result of firing the main gun (Cannon). This force simulates the recoil effect of the gun.
Higher value mkaes Bradley experience more significant movement and rotation when firing its main gun, while lower value reduces the reocil force.
