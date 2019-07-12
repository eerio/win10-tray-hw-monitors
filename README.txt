Compile this file however you like to
I placed it in my Desktop folder and then issued:
C:\Users\<user>\Desktop> C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /t:exe /out:ram.exe trayicon.cs
(the part before greater-than sign is the working dir)
Then you should see three colorful icons in your System Tray
You have to run it as admin because otherwise it's imposible to obtain CPU temperature
