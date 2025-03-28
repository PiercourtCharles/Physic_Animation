### Welcome to my animation tools project 👋

<!-- ABOUT THE PROJECT -->
## About The Project

![https://github.com/Your_Repository_Name/Your_GIF_Name.gif](https://github.com/Dr-Charlous/Un_AnimPhysic/blob/main/Assets/GitDocs/Tail.gif)

First of all this project have the objectiv to make animation tools about physic, animation, ik and more coming with time.
Now let's begin with the first point, the physic animation !

## Physic Animation

Waht is that ? It's a process where a physical model use an animate one to add rotation of each bones of the animate one to his joint as target rotation to give this physic animation aspect.

Here's what in there:
* 1 Model with the animation
* 1 Model with no animation
* The 2 scripts needed on the project (`AutoPhysicRig.cs` and `AnimatePhysicJoint.cs`)

Next let's see how to put it in work !

![https://github.com/Your_Repository_Name/Your_GIF_Name.gif](https://github.com/Dr-Charlous/Un_AnimPhysic/blob/main/Assets/GitDocs/AutoPhysicRig.png)

On the `AutoPhysicRig.cs`, as you can see there is some variables but some of them are for:
* __*Actualize*__ is a button to actualize variables during play time
* __*End branches*__ is lists of the end of each branches of the models (end of fingers, head, foot, etc...) in the physic and animate one
* __*Anim scripts config*__ are for the `AnimatePhysicJoint.cs` to config values on each ones during creation/actualization
* __*Joints config*__ are for the configurable joints on the model like for the scripts
* __*Rigibodies config*__ is like his name for the rigidbodies on the physic model

![https://github.com/Your_Repository_Name/Your_GIF_Name.gif](https://github.com/Dr-Charlous/Un_AnimPhysic/blob/main/Assets/GitDocs/AnimatePhysicJoint.png)

Next the `AnimatePhysicJoint.cs`, on this one there is less parameters, you have:
* __*Target Bone*__ used to target the animate bone to copy
* __*Is Local*__ to now if the values are from the world or local position/rotation
* __*Is Offseted*__ is to add local rotation as an offsetto the object
* __*Is Invert*__ is to invert the angle of rotation of the object



![https://github.com/Your_Repository_Name/Your_GIF_Name.gif](https://github.com/Dr-Charlous/Un_AnimPhysic/blob/main/Assets/GitDocs/Branch.gif)
Here's an exemple of that with a branch, as you see it move a bit different from the animate one and you can change it with the values on the `AutoPhysicRig.cs` script.

# Other exemples:

On those three next gifs you can see 3 differents stades of more and less physic on it
![https://github.com/Your_Repository_Name/Your_GIF_Name.gif](https://github.com/Dr-Charlous/Un_AnimPhysic/blob/main/Assets/GitDocs/Cam0.0.gif)
Less one
![https://github.com/Your_Repository_Name/Your_GIF_Name.gif](https://github.com/Dr-Charlous/Un_AnimPhysic/blob/main/Assets/GitDocs/Cam0.5.gif)
Equal
![https://github.com/Your_Repository_Name/Your_GIF_Name.gif](https://github.com/Dr-Charlous/Un_AnimPhysic/blob/main/Assets/GitDocs/Cam1.0.gif)
And more

## Now what is the next step on the project ?
The next step now is to work on ik and mostly on ik chain for the project !
