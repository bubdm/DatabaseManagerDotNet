using System;
using System.Reflection;
using System.Runtime.InteropServices;




[assembly: AssemblyTitle("RI.DatabaseManager.SQLite"),]
[assembly: AssemblyDescription("Database manager for .NET"),]
[assembly: Guid("E99BA414-C0C5-491B-A238-8880DD9DE0A5"),]

[assembly: AssemblyProduct("RotenInformatik/DatabaseManagerDotNet"),]
[assembly: AssemblyCompany("Roten Informatik"),]
[assembly: AssemblyCopyright("Copyright (c) 2017-2020 Roten Informatik"),]
[assembly: AssemblyTrademark(""),]
[assembly: AssemblyCulture(""),]
[assembly: CLSCompliant(true),]
[assembly: AssemblyVersion("0.1.0.0"),]
[assembly: AssemblyFileVersion("0.1.0.0"),]
[assembly: AssemblyInformationalVersion("0.1.0.0"),]

#if DEBUG
[assembly: AssemblyConfiguration("DEBUG"),]
#else
[assembly: AssemblyConfiguration("RELEASE")]
#if !RELEASE
#warning "RELEASE not specified"
#endif
#endif
