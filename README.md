# GnuCash2Qif
This utility exports a GnuCash Sqlite database to a QIF file format.

It has been tested with a large GNU Cash database (> 10 years of transactions) with the resulting QIF file successfully imported into Moneydance.

Note it currently only supports single account splits, if it finds multiple account splits per transaction it uses the first category to represent the total transaction. While not ideal I believe the majority of transactions are single account splits (at least mine are), but I would like to implement multiple account splits at some point.

I've used GnuCash for a number of years and am generally happy with it, although of late I have experienced some frustrations. I would like to update my accounts from an iPad rather than on a full desktop PC, yet there are no good GnuCash iPad applications that I could find, and the closed-garden model of Apple seems to discourage any official port to iOS. Also, from my experience with using other accounts packages, I like the search bar function that filters the transactions in real-time allowing you to focus in on a specific category or type of transaction. The search facilities in GnuCash feel out-dated in comparison, infact the user interface in general is starting to feel a little dated.

With these in mind I thought I would try some other personal finance package to see if they meet my requirements, but was surprised to find a lack of export options, particular for the de-facto QIF standard. This utility fills that gap.

For those who would prefer a Java version of this utility, I've also written one in another repo: https://github.com/Jason-Carter/jGnuCash2Qif

# Build Prerequesites

To build GnuCash2Qif you will require the following installed on your system:

 * Microsoft .Net Core 3.1

This should be available from Microsoft for Windows, OS X and Linux systems.


# Build Instructions
GnuCash2Qif can be build in Visual Studio, or by following these command line instructions:

```
$ git clone https://github.com/Jason-Carter/GnuCash2Qif.git
$ cd GnuCash2Qif
$ dotnet build GnuCash2Qif.sln
```

If you want to package this into a single executable, with unused modules removed (trimmed), you can use the following command:

```
$ dotnet publish -c release -r win10-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true
```

Ignore the errors on the DLL projects (I'll see if I can suppress them another time), as long as the main command line project GnuCashSql2Qif publishes then a single EXE should be created. Lookout for the following output:

```
GnuCashSql2Qif -> C:\Users\xxxx\source\repos\GnuCashSql2Qif\GnuCashSql2Qif\bin\Release\netcoreapp3.1\win10-x64\publish\
```

This is where the GnuCashSql2Qif.exe single executable will be published. A directory listing of that folder should show the following:

```
Directory: C:\Users\jjcar\source\repos\GnuCashSql2Qif\GnuCash2QifGui\bin\Release\netcoreapp3.1\win10-x64\publish


Mode                LastWriteTime         Length Name
----                -------------         ------ ----
-a----       31/01/2020     21:28       90900956 GnuCash2QifGui.exe
-a----       31/01/2020     21:27           2416 GnuCash2QifGui.pdb
```
Calling GnuCashSql2Qif.exe on its own should return the following:
```
GnuCashSql2Qif 1.0.0
Copyright (C) 2020 GnuCashSql2Qif

ERROR(S):
  Required option 'd, DataSource' is missing.
  Required option 'o, Output' is missing.

  -d, --DataSource    Required. Full path to the Sqlite file

  -o, --Output        Required. Output file

  --help              Display this help screen.

  --version           Display version information.
````
The GnuCashSql2Qif.exe can now be copied as a single file, and contains everything required to run on other (Windows 10) systems, including the .Net Core 3.1 runtime.

Note: to run the executable on other systems the ```dotnet publish``` command would require different parameters which I haven't tested.

# Prebuild Download
In the releases area of this repository you can download a single executable:

https://github.com/Jason-Carter/GnuCash2Qif/releases

This has been built using the build instructions above and contains all of the associated libraries in one executable.

# Usage
If you have built the project but not packaged it into an single executable you can run the GnuCashSql2Qif utility as follows:

```
$ dotnet GnuCashSql2Qif.dll -d "C:/accounts.sql.gnucash.sqlite" -o "C:/output.qif"
```

If you have created a single executable then you can use the following:

```
$ GnuCashSql2Qif.exe -d "C:/accounts.sql.gnucash.sqlite" -o "C:/output.qif"
```

# Options

 * `-d` Datafile for GnuCash: Must be an SQL Lite datafile (does not work with the XML files)
 * `-o` Output File which will be in QIF format
