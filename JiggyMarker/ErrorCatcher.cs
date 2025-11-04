using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiggyMarker
{
    internal static class ErrorCatcher
    {
        public static List<string[]> Errors = new List<string[]>();
        public static string[] PageSize = Directory.GetFiles(Configurator.PageSizeError);
    }
}
