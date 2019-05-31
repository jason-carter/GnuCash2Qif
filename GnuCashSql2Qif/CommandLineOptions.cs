using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace GnuCashSql2Qif
{
    class CommandLineOptions
    {
        [Option('d', "DataSource", HelpText ="Full path to the Sqlite file", Required = true)]
        public string DataSource { get; set; }

        [Option('o', "Output", HelpText = "Output file", Required = true)]
        public string Output { get; set; }
    }
}
