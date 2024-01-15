#region Using directives

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Resources;

#endregion

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("ILSpy")]
[assembly: AssemblyDescription(".NET assembly inspector and decompiler")]
[assembly: AssemblyCompany("ic#code")]
[assembly: AssemblyProduct("ILSpy")]
[assembly: AssemblyCopyright("Copyright 2011-2023 AlphaSierraPapa for the SharpDevelop Team")]

[assembly: NeutralResourcesLanguage("en-US")]

[assembly: SuppressMessage("Microsoft.Usage", "CA2243:AttributeStringLiteralsShouldParseCorrectly",
	Justification = "AssemblyInformationalVersion does not need to be a parsable version")]
