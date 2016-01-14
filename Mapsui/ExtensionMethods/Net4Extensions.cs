using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Mapsui
{
    public static class Net4Extensions
    {
        public static void Dispose(this WebResponse response)
        {
            response.Close();
        }
    }
}
