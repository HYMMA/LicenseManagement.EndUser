using Microsoft.Win32;
namespace Hymma.Lm.EndUser.Test.Tests
{
    public class DisposableTest : IDisposable
    {
      
        public void Dispose()
        {

            //recursively delete everything
            Directory.Delete(Constants.DefaultLicFileRootDir, true);

            //recreate empty folder for next run
            Directory.CreateDirectory(Constants.DefaultLicFileRootDir);
            // remove from registry
            try
            {
#pragma warning disable CA1416 // Validate platform compatibility
                Registry.CurrentUser?.DeleteSubKeyTree(Constants.RegKey);
                Registry.LocalMachine?.DeleteSubKeyTree(Constants.RegKey);
#pragma warning restore CA1416 // Validate platform compatibility
            }
            catch (Exception)
            {

            }
        }
    }
}
