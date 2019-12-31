# Musical-Flocking
## Display
#### Works On Flocking And Audio Visualization with FFTWindow.Blackman Spectrum Bands.
Here is the example Scene:
[]
## RoadMap
```mermaid
graph LR
A[Empty Project] --> B(Audio Visualisation)
A --> C(Flocking Behavior)
B --> D{Combination}
C --> D
D -- Special Effects --> E(Final Project)
```
### 3 Simple Rules of Flocking Behavior:  
- **Alignment**:  Causes a particular agent to line up with agents close by.
- **Cohesion**:  Causes agents to steer towards the "center of mass" - that is, the 	   average position of the agents within a certain radius.
- **Separation**:  Causes an agent to steer away from all of its neighbors.
