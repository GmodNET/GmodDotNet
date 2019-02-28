using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gmod.NET
{
    public interface IModule
    {
        /// <summary>
        /// Called by runtime before unload
        /// </summary>
       void CleanUp();

        /// <summary>
        /// Name of the module
        /// </summary>
        string ModuleName
        {
            get;
        }
        /// <summary>
        /// Author of the module
        /// </summary>
        string Author
        {
            get;
        }
        /// <summary>
        /// Version of module
        /// </summary>
        string Version
        {
            get;
        }
        /// <summary>
        /// Module's WebSite
        /// </summary>
        string WebSite
        {
            get;
        }
    }
}
