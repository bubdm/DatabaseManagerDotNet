using System;
using System.Reflection;
using System.Runtime.InteropServices;




[assembly: AssemblyTitle("RI.DatabaseManager.Microsoft.DependencyInjection"),]
[assembly: AssemblyDescription("Database manager for .NET"),]
[assembly: Guid("DD095019-BDF2-41E5-9E4B-3FE5C70961C9"),]

[assembly: AssemblyProduct("RotenInformatik/DatabaseManagerDotNet"),]
[assembly: AssemblyCompany("Roten Informatik"),]
[assembly: AssemblyCopyright("Copyright (c) 2017-2020 Roten Informatik"),]
[assembly: AssemblyTrademark(""),]
[assembly: AssemblyCulture(""),]
[assembly: CLSCompliant(false),]
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
