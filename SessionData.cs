using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iska
{
    /// <summary>
    /// Wraps the data that is saved in the session.
    /// 
    /// Created by: Celestine Ezeokoye. 6th Sept, 2013.
    /// </summary>
    [Serializable]
    public class SessionData
    {
        public readonly Guid SessionKey;
        public readonly DateTime DateCreated;
        public readonly object Data; //The actual data that is being persisted.

        internal SessionData(object data)
        {
            Data = data;
            SessionKey = Guid.NewGuid();
            DateCreated = DateTime.Now;
        }
    }
}
