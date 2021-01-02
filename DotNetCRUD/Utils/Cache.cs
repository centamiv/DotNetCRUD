using System.Collections.Generic;
using System.Linq;

namespace DotNetCrud.Utils
{
    internal class Cache
    {
        private Dictionary<string, Dictionary<string, string>> data;
        private static Cache singleton;
        public static Cache Singleton
        {
            get
            {
                if (singleton == null)
                {
                    singleton = new Cache();
                }
                return singleton;
            }
        }

        public Dictionary<string, string> Get(string tableName)
        {
            if (data == null)
            {
                data = new Dictionary<string, Dictionary<string, string>>();
            }
            if (string.IsNullOrEmpty(tableName))
            {
                if (data.Count > 0)
                {
                    tableName = data.Keys.First();
                }
                else
                {
                    tableName = "";
                }
            }
            if (!data.ContainsKey(tableName))
            {
                data.Add(tableName, new Dictionary<string, string>());
            }
            return data[tableName];
        }

    }
}
