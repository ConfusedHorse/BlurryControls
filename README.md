The BlurryControls library is meant to give your application a blurry look as it is known from the "AeroGlass" design

## What controls does the BlurryControls.dll provide?

1. _BlurryWindowBase_: you can inherit from BlurryWindowBase to create a blurry window
2. _BlurryTrayBase_: you can inherit from BlurryTrayBase to create a control which appears in the bottom right corner of your workspace, it is a convinient way to implement a control invoked by a tray icon (comes with an animation)
3. _BlurryDialogWindow_: this one can only be in invoked by BlurryMessageBox which provides similar functionality to the conventional MessageBox provided by Microsoft