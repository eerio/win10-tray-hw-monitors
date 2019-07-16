Compile this file however you like to

I placed it in my Desktop folder and then issued:

C:\Users\<user>\Desktop> C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /t:winexe /out:ram.exe trayicon.cs
(the part before greater-than sign is the working dir)
Then you should see three colorful icons in your System Tray

To use the CPU temperature widget we have to get the data from WMI
You can either:
-just run this script as admin and use all of its features
-disable temperature reading:
	comment out this line:
	TrayIcon batteryIcon = new TrayIcon("temp");
	it should be line 19
-try to permit reading registers from ROOT\WMI namespace to your user
^ rather not-recommended and not very secure; but if you want to, here is some info:
https://wutils.com/wmi/root/wmi/msacpi_thermalzonetemperature/

tray icons may get stuck in the hidden area and i can do nothing with it:
https://stackoverflow.com/questions/15148886/stop-auto-hiding-tray-notification-icon
sometimes its just needed to promote them by the user themselves
+they are visible normally when you run .exe by hand, not from autostart

This program wouldn't exist without these two AMAZING pieces of code:
Accepted answer:
https://stackoverflow.com/questions/24004300/batch-file-get-cpu-temperature-in-c-and-set-as-variable/24005062#24005062
And this file:
https://github.com/kas/percentage/blob/master/percentage/percentage/TrayIcon.cs
