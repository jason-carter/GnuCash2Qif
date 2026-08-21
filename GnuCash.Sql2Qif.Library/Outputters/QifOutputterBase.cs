using System;
using System.Collections.Generic;
using System.IO;

namespace GnuCash.Sql2Qif.Library.Outputters
{
    abstract public class QifOutputterBase<T>
    {
        private readonly IProgress<string> _progress;
        protected readonly StreamWriter Writer;

        public QifOutputterBase(IProgress<string> progress, StreamWriter writer)
        {
            this._progress = progress;
            this.Writer = writer;
        }

        public abstract void Write(IDictionary<string, T> accounts);

        protected bool IsReconciled(string isReconciled)
        {
            // Assuming 'cleared' accounts are reconciled
            return isReconciled.ToLower().Equals("y") ||
                    isReconciled.ToLower().Equals("c");
        }

        protected string QifAccountType(string accType)
        {
            var qifAccType = accType == "BANK" ? "Bank" :
                             accType == "CREDIT" ? "CCard" :
                             accType == "ASSET" ? "Oth A" :
                             accType == "LIABILITY" ? "Oth L" :
                             "?";

            if (qifAccType == "?")
            {
                _progress?.Report($"WARNING: Unknown account type: {accType}");
            }

            return qifAccType;
        }

        protected string QifCategoryType(string catType)
        {
            var qifCatType = catType == "INCOME" ? "I" :
                             catType == "EXPENSE" ? "E" :
                             "?";

            if (qifCatType == "?")
            {
                _progress?.Report($"WARNING: Unknown category type: {catType}");
            }

            return qifCatType;
        }
    }
}
