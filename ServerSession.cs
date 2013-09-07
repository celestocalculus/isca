using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Iska
{
    /// <summary>
    /// Core class which performs all of the session activities
    /// 
    /// Created by: Celestine Ezeokoye. 6th Sept, 2013.
    /// </summary>
    public class ServerSession
    {
        private static HashSet<SessionData> Session = new HashSet<SessionData>();
        private static int TimeToLive = 86400000, CheckEvery = 86400000; //Session has a lifetime of 1 day and cleans-up daily
        private static IStorage Storage = null; //If you don't wish to use storage, it's OK to let it be null.
        private static readonly object locker = new object();

        public static void Init(IStorage storage, int timeToLive = 86400000, int checkEvery = 86400000)
        {
            TimeToLive = timeToLive;
            CheckEvery = checkEvery;
            Storage = storage;

            /* Initialization has to happen in the main thread, no matter how long it takes.
             * To ensure that we are working on a correct version of the session data.
             */
            lock (locker)
            {
                if (Storage != null)
                {
                    var ss = Storage.Read();
                    Session = ss != null ? ss : new HashSet<SessionData>();
                }
            }
            CleanupSession();
        }

        /// <summary>
        /// Add data to session.
        /// </summary>
        /// <param name="data">The data to add to the session</param>
        /// <returns>The unique GUID used for saving the data. For identification.</returns>
        public static Guid Add(object data)
        {
            var sessData = new SessionData(data);

            lock (locker)
                Session.Add(sessData);

            LockAndAct(s => Storage.Write(s));

            return sessData.SessionKey;
        }

        /// <summary>
        /// Remove a specified data from the session.
        /// </summary>
        /// <param name="key">The unique key for the session to be removed</param>
        /// <returns>True if any data is found & removed, false otherwise</returns>
        public static bool Remove(Guid key)
        {
            var count = -1;

            lock (locker)
                count = Session.RemoveWhere(sd => sd.SessionKey == key);

            if (count > 0)
            {
                LockAndAct(s => Storage.Write(s));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Fetch a specified data from the session.
        /// </summary>
        /// <param name="key">The unique key of the data too fetch.</param>
        /// <returns>The requested data, or null if no data matches the specified key</returns>
        public static object Get(Guid key)
        {
            SessionData data = null;
            lock (locker)
                data = Session.SingleOrDefault(sd => sd.SessionKey == key);

            return data == null ? data : data.Data;
        }

        /// <summary>
        /// Runs from time to time and cleans-up the session, removing expired data.
        /// </summary>
        private static void CleanupSession()
        {
            Timer cleanup = new Timer((arg) => {
                lock (locker)
                {
                    Session.ToList().ForEach(item =>
                    {
                        if ((DateTime.Now - item.DateCreated).TotalMilliseconds >= ServerSession.TimeToLive)
                            Session.RemoveWhere(i => i.SessionKey == item.SessionKey);
                    });
                    if (Storage != null) Storage.Write(Session);
                }
            }, null, CheckEvery, CheckEvery);
        }

        /// <summary>
        /// Helper function to lock on required resources and perform specified action.
        /// </summary>
        /// <param name="act">Action to perform</param>
        private static void LockAndAct(Action<HashSet<SessionData>> act)
        {
            new Task(() =>
            {
                lock (locker)
                {
                    if (Storage != null)
                        act(Session);
                }
            }).Start();
        }
                    
    }
}
