# AllJoynDotNet
A .NET wrapper for the AllJoyn C Library

##### Note: This is very much a work in progress!

Features:
  - Code generator that auto-generates the Interop from the AllJoyn C-API, including full code-summaries
  - Library for Windows Universal, Windows Desktop, Xamarin-Android, and Xamarin-iOS

What's not working:
  - Besides limited creation of bus attachments, no classes has been done for the rest yet
  - Generated interop code is mostly untested, but so far seems to work in most places
  - Xamarin-Android currently doesn't work at all (can't find the library)
