using Hymma.Lm.EndUser.Utilities;
using Microsoft.Win32;
using System.Collections.Generic;
namespace Hymma.Lm.EndUser.Registrars
{
    /// <summary>
    /// computer registrar that saves computer-id on HKCU and HKLM in registry on windows
    /// </summary>
    public static class ComputerRegister
    {

        /// <summary>
        /// Reads from <see cref="Registry.LocalMachine"/> and on error reads <see cref="Registry.CurrentUser"/>
        /// </summary>
        /// <param name="computerId"></param>
        /// <returns></returns>
        public static bool TryRead(out string computerId)
        {
            return MainRegistryKeyInHKLM.TryReadFrom(Constants.COMPUTER_KEY, Constants.COMPUTER_ID, out computerId);
        }

        /// <summary>
        /// writes to <see cref="Registry.CurrentUser"/> and <see cref="Registry.LocalMachine"/> in order
        /// </summary>
        /// <param name="computerId"></param>
        /// <returns></returns>
        public static bool TryWrite(string computerId)
        {
            return MainRegistryKeyInHKLM.TryWriteTo(Constants.COMPUTER_KEY, new KeyValuePair<string, string>(Constants.COMPUTER_ID, computerId));
        }
    }
}
