using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;

namespace GnuCash.Sql2Qif.Library.DAL.Mappers
{
    abstract public class MapperBase<TKey, TValue>
    {
        private readonly ILogger logger;

        public MapperBase(ILogger<TValue> logger)
        { 
            this.logger = logger;
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
                    logger.LogWarning($"Duplicate key ({mapped.Key}) for ({mapped.Value}), ignoring ");
                }
                catch (Exception ex)
                {
                    logger.LogWarning($"Cannot add ({mapped.Value}) due to ({ex.Message}), ignoring ");
                }
            }
            return dict;
        }
    }
}
