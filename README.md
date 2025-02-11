# AssetOrganizer
An (actually good) asset organizer for unity/vrchat workflows. Organizes assets based on context, such as materials by shader.

To download, click the green "Code" button, then download zip (this is effectively cloning), afterwards open the zip file in a program like WinRAR and drag the folder within into your assets folder.

## How things are organized
| Asset Type | Context 
| :------------------- | :----------:
| Animations             | Scene Name > Avatar GameObject Name > Asset Type > Animator Layer > Animator State >
| Animators              | Scene Name > Avatar GameObject Name > Asset Type > Animator VRCAvatarDescriptor Category >
| Materials              | Scene Name > Avatar GameObject Name > Asset Type > Shader Name >
| Meshes              | Scene Name > Avatar GameObject Name > Asset Type > Static or Dynamic (SkinnedMeshRenderer or not) > GameObject Name > 
| Scenes              | Scene Name > Avatar GameObject Name > Asset Type >
| Textures              | Scene Name > Avatar GameObject Name > Asset Type > Material Name >
| VRCSDK              | Scene Name > Avatar GameObject Name > Asset Type >

## Contributing
Please read and understand the existing asset type modules, and use one as a template. This is essential for predictable outcomes to happen.
Once ready, make a pull request.

## Donate
You can find links on the right to donate if you like.
