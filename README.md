# DangerDodger
A plugin for ExileBuddy

## Contributing
To contribute to this plugin, start by creating a fork of this project. 
Once you have a fork, clone the fork on your computer.
Then, create a new Class Library project in Visual Studio and add the following references to the project:
- The exile buddy executable
- RemoteASMNative.dll
- log4net.dll
- RemoteASM.dll
- GreyMagic.dll
- Newtonsoft.Json.dll
- System.Xaml

Once this is done, add the files in your cloned folder as reference is your project.
To do this, right click on your project, select *Add* -> *Existing Item...* -> *{highlight the files}* -> *Add as link*

You can now work on the plugin.

Before starting your work, I recommend doing a branch. You can do this with the following git command: 

`git checkout -b myBranchName`

Working on a branch will simplify the processing of Pull Requests. It will allow you to make many independant Pull Requests.

To submit your work, create a Pull Request. Pull Requests can be created through the web UI. Simply go to https://github.com/Buddyfu/DangerDodger/ and click on the *New Pull Request* button. Once you have made a PR, notify me on ExileBuddy's forum. I will take a look at it and integrate it into the project.


