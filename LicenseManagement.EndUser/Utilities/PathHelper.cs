using System.IO;
using System.Linq;
using System.Text;

namespace Hymma.Lm.EndUser.Utilities
{
    internal static class PathHelper
    {
        /// <summary>
        /// removes invalid file name characters from file name
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>string without invalid file name chars</returns>
        internal static string RemoveInvalidFileNameChars(string fileName)
        {
            var sb = new StringBuilder();
            var invChars = Path.GetInvalidFileNameChars();
            for (int i = 0; i < fileName.Length; i++)
            {
                if (!invChars.Contains(fileName[i]))
                {
                    sb.Append(fileName[i]);
                }
            }
            return sb.ToString();
        }
    }
}
