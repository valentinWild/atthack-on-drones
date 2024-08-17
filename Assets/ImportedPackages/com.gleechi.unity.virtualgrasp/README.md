VirtualGrasp (VG) is a software development kit (SDK) developed from over eight years of research in robotics, healthcare and industrial applications.
VG SDK provides a set of tools to make hand-object interactions in VR natural and immersive. The three main benefits of using VG are:
* synthesizing natural looking grasp configurations on hands during [grasp interaction](https://docs.virtualgrasp.com/grasp_interaction.1.1.0.html) in a VR application, and
* easy setup of interactive behaviors of an object (through [object articulation](https://docs.virtualgrasp.com/object_articulation.1.1.0.html)) when hand [grasps](https://docs.virtualgrasp.com/grasp_interaction.1.1.0.html) or [pushes](https://docs.virtualgrasp.com/push_interaction.1.1.0.html) the object. 
* neither grasp synthesis nor [object articulation](https://docs.virtualgrasp.com/object_articulation.1.1.0.html) requires physical setup of the objects or avatars, while both features integrate seamlessly with any physical objects or environment.

Synthesizing grasps and creating interactive behaviors are the two features that closely link to each other. 
To have intuitive object grasp interaction experiences, just synthesizing the natural looking 
grasp configuration is not enough. How the hand and object moves just before and after grasping needs to be carefully handled, which is solved by VG's [object articulation](https://docs.virtualgrasp.com/object_articulation.1.1.0.html) feature.

Note that, while the interactive behavior is provided out of the box from the VG SDK, 
to achieve natural looking grasp configurations in runtime
requires a preprocessing step called [object baking](https://docs.virtualgrasp.com/object_baking.1.1.0.html).

VG is hardware agnostic and can create natural [grasp interactions](https://docs.virtualgrasp.com/grasp_interaction.1.1.0.html) with any kind of controllers (or sensors). 
You can find more details in [controllers](https://docs.virtualgrasp.com/controllers.1.1.0.html) page.

As a general guideline to this site:

* [Tutorials](https://docs.virtualgrasp.com/unity_get_started_installation.1.1.0.html) take you by the hand through a series of steps to learn how to use VirtualGrasp.
* [Explanations](https://docs.virtualgrasp.com/controllers.1.1.0.html) lead you to learn about fundamental concepts in VirtualGrasp.
* [How-To Guides](https://docs.virtualgrasp.com/unity_component_myvirtualgrasp.1.1.0.html) are recipes that guide you through the components involved in addressing key problems and use-cases.
* [References](https://docs.virtualgrasp.com/virtualgrasp_unityapi.1.1.0.html) contain technical reference for VirtualGrasp APIs and components as well as release notes. They describe how it works and how to use it,
 but assume that you have a basic understanding of key concepts in [Explanations](https://docs.virtualgrasp.com/controllers.1.1.0.html).
