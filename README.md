# Hololens

## Installation

Unfortunately the configuration is using absolute paths. Before you open the project open `Hololens\UnituCommon.props` and set all paths correctly. Afterwards you can open the project.

## Deploy

First, make sure that the `x86` configuration is selected/
You can either deploy to device over Wi-Fi or USB. **I advise to use USB**, it is much faster.

You can also use Hololens Simulator. It is somehow slow, but works. You need to install previous version. If there are any questions, just follow this tutorial which explains everything:

https://docs.microsoft.com/en-us/windows/mixed-reality/holograms-100

On the same page, in the left menu you have other cool tutorials.

# Server

## Prerequisites

If you want to build the C++ project, you need to have following prerequisites installed:

1. ZED SDK
2. CUDA 9.2
3. Copy `OpenCV` to `C:\Libraries\opencv`. Make sure that following paths exist and contains the include files:
  1. `C:\Libraries\opencv\build\include`
  2. `C:\Libraries\opencv\build\x64\vc15\lib\opencv_world340d.lib`

  Please use this directory, so that we do not have to change it in every setup. If you do need to change it, let me know and we can discuss a better solution.
  
## Installation

Just copy the `opencv_world340d.dll` to `Server\bin` directory. Project is set up to use debug solution only at this moment.
