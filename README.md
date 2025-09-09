### Welcome to my physic animation tool project 👋

<!-- ABOUT THE PROJECT -->
## About The Project

![https://github.com/Your_Repository_Name/Your_GIF_Name.gif](https://github.com/Dr-Charlous/Un_AnimPhysic/blob/main/Assets/GitDocs/Tail.gif)

First of all this project have the objectiv to make animation tool about physic and animation.

## Setup

What are we beginning with ? First the script `AutoPhysicRig.cs` need to be in the seen on an object.

Here's what we need next:
* 1 Model with the animation
* 1 Model with physic (without animations)

Next let's see how to make it work !

![https://github.com/Your_Repository_Name/Your_GIF_Name.gif](https://github.com/Dr-Charlous/Un_AnimPhysic/blob/main/Assets/GitDocs/Script.png)

On the script:
* __*Root*__ is for the root of the model
* __*End chains*__ are lists of the end of each branch of the models (end of fingers, head, foot, etc...) in the physic and animate one
* __*Is Free Motion*__ free the root in space and __*Use Phyic*__ define if the model have physic or not (if change in playmode need to click on `Actualize`)
<br><br>
* __*Joints config*__ are for the configurable joints values apply on the model by default if not set before playmode
* __*Rigibodies config*__ is for the rigidbodies values on the physical model
* __*Blend Ragdoll Value*__ Litteraly to blend ragdoll 
<br><br>
* __*Sub Parts*__ Same thing as the principal script but apply from A to B in a part of the model
  ![https://github.com/Your_Repository_Name/Your_GIF_Name.gif](https://github.com/Dr-Charlous/Un_AnimPhysic/blob/main/Assets/GitDocs/SubParts.png)
<br><br>
* __*Buttons*__ One to `Setup` the model before playmode for specific values, `Update` to update values and `Clean` to delete components setup before
  `SubGroups` is to search elements between A and B outside playmode
