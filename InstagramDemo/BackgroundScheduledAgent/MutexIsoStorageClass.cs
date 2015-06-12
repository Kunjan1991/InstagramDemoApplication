using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OnesocialBackgroundAgent
{
    public static class MutexedIsoStorageFile
    {
        //named mutex
        private static Mutex Mutex = new Mutex(false, "BackgroundAgent");

        //name of isolated storage file
        private const String IsoStorageDateFile = "BackgroundAgentOnesocial.txt";

        //read iso storage
        //debug.writeline lets me "see" agent working in VS output window
        public static IsoStorageData Read()
        {
            IsoStorageData IsoStorageData = new IsoStorageData();
            Mutex.WaitOne();
            try
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                using (var stream = new IsolatedStorageFileStream(IsoStorageDateFile, FileMode.OpenOrCreate, FileAccess.Read, store))
                using (var reader = new StreamReader(stream))
                {
                    if (!reader.EndOfStream)
                    {
                        var serializer = new XmlSerializer(typeof(IsoStorageData));
                        IsoStorageData = (IsoStorageData)serializer.Deserialize(reader);
                    }
                }
            }
            finally
            {
                Mutex.ReleaseMutex();
            }
            return IsoStorageData;
        }
        //write iso storage
        //debug.writeline lets me "see" agent working in VS output window
        public static void Write(IsoStorageData data)
        {
            // persist the data using isolated storage
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            using (var stream = new IsolatedStorageFileStream(IsoStorageDateFile,
                                                              FileMode.Create,
                                                              FileAccess.Write,
                                                              store))
            {
                var serializer = new XmlSerializer(typeof(IsoStorageData));
                serializer.Serialize(stream, data);
            }
        }

    }
}
