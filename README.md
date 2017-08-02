# J3DAnim 

J3DAnim is a tool used to convert Maya's ASCII animation format to Nintendo's J3D animation format.

**Guide:**

**1.)** Have your model open in Maya, fully animated and ready for export (In this example, I'll be using Mario's model from Super Mario Sunshine with his death animation) 


**2.)** Make sure you have a keyframe for both the start and ending frames of the animation for every bone.


**3.)** Go to the scene hierarchy and highlight every joint.
![alt text](https://s2.postimg.org/rs9l3r3nt/sceneh.png)

**4.)** Once you've done that, it's time to export! Proceed by going to **File -> Export Selection** and set **Files of type** to **animExport**. Export your .anim and open up J3DAnim.

**5.)** After opening up J3DAnim, open your .anim file and BMD model. Select the loop mode on the right and then export! If everything turns out good, a BCK file should be exported in the directory of the .anim file.


This can be used for all sorts of things, so I hope you use this wisely! :}
