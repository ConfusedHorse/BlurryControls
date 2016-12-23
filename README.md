The BlurryControls library is meant to give your application a blurry look as it is known from the "AeroGlass" design.

! _It is only meant to be used on Windows 10 since the used strategy does not work on prior versions of Windows._
Feel free to have a look at the Wiki for further information and usage instructions (it's actually quite self explanatory) :)

## Which controls does the BlurryControls.dll provide

1. _BlurryWindow_: you can inherit from BlurryWindow to create a blurry window
2. _BlurryTray_: you can inherit from BlurryTray to create a control which appears (by default) in the bottom right corner of your workspace, it is a convinient way to implement a control invoked by a tray icon (comes with an animation)
3. _BlurryDialogWindow_: this one can only be in invoked by BlurryMessageBox which provides functionality similar to the conventional MessageBox provided by Microsoft
3. _BlurryImage_: a conventional wpf Image with additional functionality to blur it dynamically

## What else comes with it

In the solution you will find a project called BlurryWindowInvoker. It provides a short presentation of what the BlurryControls.dll is capable of.
