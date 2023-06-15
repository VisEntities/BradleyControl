# From bland to badass!
Sick of the same old Bradley routine? Inject this armored beast with some steroids and level it up with the intelligence and strength it deserves! No more weaknesses to be exploited or predictable patterns to memorize. It's time to watch Bradley take revenge on all those who thought they had it all figured out.

![](https://i.imgur.com/Jvxewu8.png)

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
Determines whether or not fireballs will be created for each crate that spawns after Bradley's destruction. If enabled, every crate will be set on fire, making it harder for players to collect the loot without taking damage.


### Spread Chance
Determines the probability of a fireball spreading and creating new ones. A higher value will increase the chance of the fireball spreading further, while a lower value will make the fireball less likely to spread.

Be mindful of the impact on server performance when making adjustments to this setting.

### Spread At Lifetime Percent
Determines at what point in the fireball's lifetime it'll begin to spread additional fireballs.

### Maximum Speed
Determines the maximum torque applied to Bradley's wheels, affecting how fast it can move forward and backward.

### Spin Speed
Determines how quickly Bradley can turn or spin.

A higher value will result in faster spinning, making Bradley more agile and responsive. A lower value, however, will make it harder for Bradley to maneuver, making it an easier target for enemy fire.

### Brake Force
Determines the strength of Bradley's brakes, which impacts its deceleration rate and stopping distance.

Higher values allow for faster deceleration and shorter stopping distances, helping Bradley stay on its path more accurately. However, this may also lead to slower movement. On the other hand, lower values result in slower deceleration, which could make Bradley slip off its path more often.

### Engagement Range
Determines the range of visibility at which Bradley can interact with players and engage them with its weapons.

### Target Search Range
Determines the radius around Bradley within which it can see and track potential targets. A higher value allows Bradley to track targets at a greater distance, while a lower value limits the detection and tracking range.

### Memory Duration
Determines how long Bradley remembers a target after it has last seen them.

A higher value makes Bradley remembers targets for a longer duration, even if they go out of sight. This is especially useful when engaging targets that use cover or attempt to hide from Bradley's line of sight. Contrariwise, a lower value causes Bradley to forget targets more quickly once they go out of sight, limiting its ability to engage them.

### Time Between Bursts
Determines the time between each shot fired by the coaxial gun. A higher value will result in longer periods of time between shots, while a lower value will result in more rapid and aggressive firing.

### Maximum Shots Per Burst
Determines the maximum number of shots that can be fired in a single burst by the coaxial gun.

A higher value allows for more shots to be fired in quick succession. On the contrary, a lower value means that fewer shots can be fired in a single burst. 

### Bullet Damage
Determines the base amount of damage that is dealt by each bullet fired from the coaxial gun.

### Recoil Intensity
Determines the amount of force applied to Bradley's body as a result of firing its cannon gun, simulating the recoil effect of the gun.

A higher value means that Bradley will experience more significant movement and rotation when firing, while a lower value reduces the amount of recoil force.

Note that too much recoil can impact Bradley's accuracy and stability, while too little can make it less effective in combat situations.

------------

## Keep the mod alive

Creating plugins is my passion, and I love nothing more than exploring new ideas and bringing them to the community. But it takes hours of work every day to maintain and improve these plugins that you have come to love and rely on. 

With your support on [Patreon](https://www.patreon.com/VisEntities), you're  giving me the freedom to devote more time and energy into what I love, which in turn allows me to continue providing new and exciting content to the community.

![](https://i.imgur.com/qmv03TS.png)

A portion of the contributions will also be donated to the uMod team as a token of appreciation for their dedication to coding quality, inspirational ideas, and time spent for the community.

-------

## Credits
* Originally created by **Mattparks**, up to version 0.2.3
* Completely rewritten from scratch and maintained to present by **Dana**.