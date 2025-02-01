using System;
using System.Collections.Generic;
using System.Data;

namespace GnuCash.Sql2Qif.Library.DAL.Mappers
{
    abstract public class MapperBase<TKey, TValue>
    {
        private readonly IProgress<string> progress;

        public MapperBase(IProgress<string> progress)
        { 
            this.progress = progress;
        }

        protected abstract KeyValuePair<TKey, TValue> Map(IDataRecord record);
        public Dictionary<TKey, TValue> MapAll(IDataReader reader)
        {
            Dictionary<TKey, TValue> dict = new();
            while (reader.Read())
            {
                KeyValuePair<TKey, TValue> mapped = Map(reader);
                try
                {
                    dict.Add(mapped.Key, mapped.Value);
                }
                catch (ArgumentException)
                {
                    progress?.Report($"WARNING: Duplicate key ({mapped.Key}) for ({mapped.Value}), ignoring ");
                }
                catch (Exception ex)
                {
                    progress?.Report($"WARNING: Cannot add ({mapped.Value}) due to ({ex.Message}), ignoring ");
                }
            }
            return dict;
        }
    }
}
