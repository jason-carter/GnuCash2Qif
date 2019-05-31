# GnuCash2Qif
Convert GnuCash Sqlite database to QIF format

This is still a WIP and not yet ready for general use. Major refactors are happening every time I add or change something at the moment, but feel free to play around with it.

## To Do

 - ~~Extract and~~ Output transaction functionality
 - Output to a file (currently outputs to System Out)
 - Add warnings/errors to the output (pass back to gui layer via event)
 - Add the CLI Options project properly (then can improve this Readme.md with better usage instructions
 - Add a mapping function for categories? Not sure about this
 - Consider a WPF UI (now that MS are supporting it going forward)
 - Add Unity / DI and Unit Tests (would normally have done this first if it wasn't experimental)
 
## Longer-term Possibilities
These posibilities simply interest me so might explore them later once this project is mature enough.

 - Add [journal](https://hledger.org/journal.html) as an output format, and explore [Plain Text Accounting](https://plaintextaccounting.org/)
 - Rewrite in functional language (Haskell or Erlang, probably not F# as want to try something not from MS)
