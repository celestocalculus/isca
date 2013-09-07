using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iska
{
    /// <summary>
    /// Declares the methods required for persisting the data to storage.
    /// 
    /// This interface should be extended and used to tell Iska where and how
    /// to serialize and deserialize the required data.
    /// 
    /// Created by: Celestine Ezeokoye. 6th Sept, 2013.
    /// </summary>
    public interface IStorage
    {
        /// <summary>
        /// Write session data to persistent store.
        /// </summary>
        /// <param name="data">The whole session data to write</param>
        void Write(HashSet<SessionData> data);

        /// <summary>
        /// Read session data from persistent store.
        /// </summary>
        /// <returns>The whole session data read</returns>
        HashSet<SessionData> Read();
    }
}
