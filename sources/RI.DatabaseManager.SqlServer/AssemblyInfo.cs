using System;
using System.Reflection;
using System.Runtime.InteropServices;




[assembly: AssemblyTitle("RI.DatabaseManager.SqlServer"),]
[assembly: AssemblyDescription("Database manager for .NET"),]
[assembly: Guid("A818288C-1A73-4B2D-9488-1DB535DF0C4F"),]

[assembly: AssemblyProduct("RotenInformatik/DatabaseManagerDotNet"),]
[assembly: AssemblyCompany("Roten Informatik"),]
[assembly: AssemblyCopyright("Copyright (c) 2017-2020 Roten Informatik"),]
[assembly: AssemblyTrademark(""),]
[assembly: AssemblyCulture(""),]
[assembly: CLSCompliant(true),]
[assembly: AssemblyVersion("1.0.0.0"),]
[assembly: AssemblyFileVersion("1.0.0.0"),]
[assembly: AssemblyInformationalVersion("1.0.0.0"),]

#if DEBUG
[assembly: AssemblyConfiguration("DEBUG"),]
#else
[assembly: AssemblyConfiguration("RELEASE")]
#if !RELEASE
#warning "RELEASE not specified"
#endif
#endif
