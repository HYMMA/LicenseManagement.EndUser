using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("LicenseManagement.EndUser")]
[assembly: AssemblyDescription("End-user SDK for license-management.com")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Hymma")]
[assembly: AssemblyProduct("LicenseManagement.EndUser")]
[assembly: AssemblyCopyright("Copyright © 2025")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("ae18bd8e-bffe-4ba1-8e65-f4b0accb54bf")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("3.0.0.0")]
[assembly: AssemblyFileVersion("3.3.0.0")]

// Test project is signed with the same HymmaLm.snk so it can access internals (the *ApiEndPoint
// classes, the chain handlers, etc.). Public key extracted with `sn -p HymmaLm.snk` then dumped as hex.
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("LicenseManagement.EndUser.Test, PublicKey=002400000480000094000000060200000024000052534131000400000100010021594b6aa734266e5c22ad9d296a65fc99154cd4c4296828a3f2d8652fb76dac1ca60962ae6b16bf01fb3b173d85d1073d5ac9dcf40bb717d1b18786edc902f842b2be8897fd0704778d60e2fe618aef579fad24bbdedfeea92b09a200729512f6716cf8341355efbecd9f26b65e59f31f2aa19627b60ead56faca79b0d738bd")]
