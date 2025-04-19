### Welcome to my animation tools project 👋

<!-- ABOUT THE PROJECT -->
## About The Project

![https://github.com/Your_Repository_Name/Your_GIF_Name.gif](https://github.com/Dr-Charlous/Un_AnimPhysic/blob/main/Assets/GitDocs/Tail.gif)

First of all this project have the objectiv to make animation tools about physic, animation, ik and more coming with time.
Now let's start with the first point, the physical animation !

## Physic Animation

What is it ? It's a process where a physical model uses an animated one to add the rotation of each bone of the animated one to its joint as a target rotation to give this physic animation aspect.

Here's what's in there:
* 1 Model with the animation
* 1 Model without the animation
* The 2 scripts needed for the project (`AutoPhysicRig.cs` and `AnimatePhysicJoint.cs`)

Next let's see how to make it in work !

![https://github.com/Your_Repository_Name/Your_GIF_Name.gif](https://github.com/Dr-Charlous/Un_AnimPhysic/blob/main/Assets/GitDocs/AutoPhysicRig.png)

On the `AutoPhysicRig.cs`, as you can see, there are some variables, but some of them are for:
* __*Actualise*__ is a button to actualise variables during play time
* __*End branches*__ are lists of the end of each branch of the models (end of fingers, head, foot, etc...) in the physic and animate one
* __*Anim scripts config*__ are for the `AnimatePhysicJoint.cs` to configure values on each during creation/actualisation
* __*Joints config*__ are for the configurable joints on the model like for the scripts
* __*Rigibodies config*__ is like its name for the rigidbodies on the physical model

![https://github.com/Your_Repository_Name/Your_GIF_Name.gif](https://github.com/Dr-Charlous/Un_AnimPhysic/blob/main/Assets/GitDocs/AnimatePhysicJoint.png)

Next is the `AnimatePhysicJoint.cs`, on this one there are less parameters, you have:
* __*Target Bone*__ used to target the animated bone to copy
* __*Is Local*__ to specify if the values are from the world or local position/rotation
* __*Is Offseted*__ is to add local rotation as an offset to the object
* __*Is Invert*__ to invert the rotation angle of the object



![https://github.com/Your_Repository_Name/Your_GIF_Name.gif](https://github.com/Dr-Charlous/Un_AnimPhysic/blob/main/Assets/GitDocs/Branch.gif)
Here's an example of this with a branch, as you can see it moves a bit differently from the animated one and you can change it with the values in the `AutoPhysicRig.cs` script.

# More exemples:

In these next three gifs you can see 3 different stages of more and less physics on it
![https://github.com/Your_Repository_Name/Your_GIF_Name.gif](https://github.com/Dr-Charlous/Un_AnimPhysic/blob/main/Assets/GitDocs/Cam0.0.gif)
Less one
![https://github.com/Your_Repository_Name/Your_GIF_Name.gif](https://github.com/Dr-Charlous/Un_AnimPhysic/blob/main/Assets/GitDocs/Cam0.5.gif)
Equal
![https://github.com/Your_Repository_Name/Your_GIF_Name.gif](https://github.com/Dr-Charlous/Un_AnimPhysic/blob/main/Assets/GitDocs/Cam1.0.gif)
And more

## So what is the next step in the project ?
The next step now is to work on the ik and especially the ik chain for the project !
