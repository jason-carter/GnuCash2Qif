# GnuCash2Qif
Convert GnuCash Sqlite database to QIF format

This is still a WIP and not yet ready for general use. Major refactors are happening every time I add or change something at the moment, but feel free to play around with it.

## To Do

 - ~~Extract and Output transaction functionality~~
 - Output to a file (currently outputs to System Out) : Probably need to move the QIF output functions from the BLL objects to an output format object to allow for larger resultsets being returned
 - ~~Add warnings/errors to the output (pass back to gui layer via event)~~
 - ~~Add the CLI Options project properly~~
 - Change this Readme.md with usage instructions (and move this roadmap - or what will be a roadmap - to the wiki)
 - Add a mapping function for categories? Not sure about this
 - Consider a WPF UI (now that MS are supporting it going forward)
 - Add Unity / DI and Unit Tests (would normally have done this first if it wasn't experimental)

## Longer-term Possibilities
These ideas interest me so I might explore them later once this project is mature enough.

 - Is it possible to do this in pure SQL? Was nearly there at the start of this project but merging the double entries proved difficult.
 - Add [journal](https://hledger.org/journal.html) as an output format, so I can explore [Plain Text Accounting](https://plaintextaccounting.org/)
 - Rewrite in functional language (Haskell or Erlang are the ones's I know and would like to learn deeper)
