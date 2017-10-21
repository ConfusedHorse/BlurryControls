Welcome to the BlurryControls wiki!

The BlurryControls library is meant to give your application a blurry look as it is known from the _AeroGlass_ design.
Please consider that this library runs on Windows 10 only!

# BlurryControls.dll 
Get it on NuGet! `PM> Install-Package BlurryControls`

#### It provides the following functionality:

1. _BlurryWindow_: you can inherit from BlurryWindow to create a blurry window
2. _BlurryTray_: you can inherit from BlurryTray to create a control which appears (by default) in the bottom right corner of your workspace, it is a convinient way to implement a control invoked by a tray icon (comes with an animation)
3. _BlurryDialogWindow_: this one can only be in invoked by BlurryMessageBox which provides functionality similar to the conventional MessageBox provided by Microsoft
4. _BlurryImage_: a conventional WPF Image with additional functionality to blur it dynamically
5. a small preset of style suggestions

# Have a look at the sample application

![](https://i.imgur.com/GxXtTsh.jpg)

The solution provides a small impression of what you can do with this library.

## How to use the controls
### BlurryWindow
inherits from System.Windows.Window

### Missing functionality:
- resize when TaskBar is **not** oriented at the bottom of the screen with a multi-monitor setup (works great on primary monitor, or aero snap though)

### Code example for common usage:

C#:
```
var blurryWindow = new BlurryWindow();
blurryWindow.Show();
//alternatively blurryWindow.ShowDialog() to block calling instance
```
Note: The usual method to invoke is `StartupUri="BlurryExampleWindow.xaml"` in your App.xaml.cs

XAML:
```
<windows:BlurryWindow x:Class="SuperCoolExampleNamespace.BlurryExampleWindow"
                      xmlns:windows="clr-namespace:BlurryControls.Windows;assembly=BlurryControls"
                      xmlns:control="clr-namespace:SuperCoolExampleNamespace"
                      Strength="0.75">
    <Grid x:Name="SuperCoolExampleContent">
        <!-- content goes here -->
    
    </Grid>
</windows.BlurryWindow>
```
Note: When applying a custom brush to the Background property, the Strength will be applied to it. The default value of Strength is 0.75

### Code example for additional MenuBar items:

XAML:
```
<windows:BlurryWindow.AdditionalMenuBarButtons>
    <internals:ButtonCollection>
        <Button x:Name="ExampleButton1"
                Click="ExampleButton_OnClick">
            <Button.Content>
                <Image Source="../Resources/ExampleImage.jpeg"/>
            </Button.Content>
        </Button>
        <Button x:Name="ExampleButton2"
                Click="ExampleButton_OnClick"
                Foreground="White"
                Content="ClickMe"/>
    </internals:ButtonCollection>
</windows:BlurryWindow.AdditionalMenuBarButtons>
```
Note: a style fitting the existing buttons will be applied automagically

C#:
```
private void Test_OnClick(object sender, RoutedEventArgs e)
{
    BlurryMessageBox.Show("Hello there!");
}
```

### Additional parameters:
- IsResizable
- IsMenuBarVisible
- Strength
- CloseOnIconDoubleClick
- AdditionalMenuBarButtons
- HorizontalTitleAlignment

## BlurryTray

### Code example for common usage:

C#:
```
var tray = new InvokedTrayWindow();
tray.Show();
```
Note: You can also use `tray.ShowDialog` to block interaction with the calling instance as long as the BlurryControl is activated

XAML:
```
<windows:BlurryTray x:Class="SuperCoolExampleNamespace.BlurryExampleTray"
                      xmlns:windows="clr-namespace:BlurryControls.Windows;assembly=BlurryControls"
                      xmlns:control="clr-namespace:SuperCoolExampleNamespace"
                      Strength="0.75"
                      Duration="5"
                      DeactivationDuration="500">
    <Grid x:Name="SuperCoolExampleContent">
        <!-- content goes here -->
    
    </Grid>
</windows.BlurryTray>
```

### AdditionalParameters:
- Strength
- DeactivatesOnLostFocus
- Duration
- ActivationDuration
- DeactivationDuration

## BlurryDialogWindow
(is private and can only be invoked by the static class BlurryMessageBox)

### Code example for common usage:

C#:
```
var mainWindow = Application.Current.MainWindow;
var messageHeaderText = "ExampleDialog";
var messageContentText = $"This is a dialog owned by {mainWindow.Title}";
var result = BlurryMessageBox.Show(mainWindow, messageContentText, 
                 messageHeaderText, BlurryDialogButton.OkCancel, BlurryDialogIcon.Information);

switch (result)
{
    case BlurryDialogResult.Ok:
        Debug.WriteLine($"The result is {BlurryDialogResult.Ok}");
        break;
    case BlurryDialogResult.Cancel:
        Debug.WriteLine($"The result is {BlurryDialogResult.Cancel}");
        break;
    case BlurryDialogResult.None:
        Debug.WriteLine($"The result is {BlurryDialogResult.None}");
        break;
    default:
        throw new ArgumentOutOfRangeException();
}
```

### Additional overloads:
```
Show(string messageBoxText, double strength = 0.5);
Show(string messageBoxText, string caption, double strength = 0.5);
Show(Window owner, string messageBoxText, double strength = 0.5);
Show(string messageBoxText, string caption, BlurryDialogButton button, double strength = 0.5);
Show(Window owner, string messageBoxText, string caption, double strength = 0.5);
Show(string messageBoxText, string caption, BlurryDialogButton button, BlurryDialogIcon icon, double strength = 0.5);
Show(Window owner, string messageBoxText, string caption, BlurryDialogButton button, double strength = 0.5);
Show(Window owner, string messageBoxText, string caption, BlurryDialogButton button, BlurryDialogIcon icon, double strength = 0.5);
```

## BlurryImage
inherits System.Windows.Controls.Image

### Code example for common usage:

XAML:
```
<windows:BlurryImage x:Name="BlurryExampleImage" 
                     Source="../Resources/ExampleImage.jpeg"
                     BlurRadius="50"/>
```
Note: The BlurRadius property is bindable and can be adjusted dynamically.

### Additional parameters:
- BlurRadius

## Global styling

### BlurryResources.xaml

Note: For global style overrides you can add the following line to your local resources. It provides a small preset of style suggestions, as it can be seen in the sample application.

XAML:
```
<Application.Resources>
    <ResourceDictionary Source="pack://application:,,,/BlurryControls;component/Themes/Generic.xaml" />
</Application.Resources>
```
