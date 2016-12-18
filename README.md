Raspberry Pi Resin Printer Host
===============

this project started to be a hobby project to see if its possible to control a resin 3d printer via the raspberry pi running Windows IOT
One of the main reasons to do this is the development environment is completely integrated from within the IDE you can write the code, compile it and debug it.

This project is a copy from the blinkyserver sample from Microsoft and addapted and added many new features.
the front end backend communication is one of the things that is interesting
[Documentation for this sample](https://developer.microsoft.com/en-us/windows/iot/samples/serialuart) 

## WCF
This project also has WCF incorperated into it, i've tried to setup a service on the raspberry pi but sofar this was unsuccessfull.

## Arduino Connection
Microsoft.Maker.Serial is being used to connect to the arduino. G-Code commands are send via this interface to the Arduino.

## Architecture

the blinkyserver works with 2 seperate app's
one app handles all the http work the other handles all the 3d printer work
the 2 apps use appServiceConnection to communicate with each other.

## appServiceConnection
Represents a connection to the endpoint for an app service. App services enable app-to-app communication by allowing you to provide services from your Universal Windows app to other Universal Windows apps.