using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConfigFileManager.Functions
{
    public static class Utility
    {
        /// <summary>
        /// Print out a string of bytes as the actual integer numbers.
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        public static string PrintBytes(this byte[] byteArray)
        {
            var sb = new StringBuilder("new byte[] { ");
            for (var i = 0; i < byteArray.Length; i++)
            {
                var b = byteArray[i];
                sb.Append(b);
                if (i < byteArray.Length - 1)
                {
                    sb.Append(", ");
                }
            }
            sb.Append(" }");
            return sb.ToString();
        }
    }
}
