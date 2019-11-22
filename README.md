# GnuCash2Qif
This utility exports a GnuCash Sqlite database to a QIF file format.

It has been tested with a large GNU Cash database (> 10 years of transactions) with the resulting QIF file successfully imported into Moneydance.

Note it currently only supports single account splits, if it finds multiple account splits per transaction it uses the first category to represent the total transaction. While not ideal I believe the majority of transactions are single account splits (at least mine are), but I would like to implement multiple account splits at some point.

I've used GnuCash for a number of years and am generally happy with it, although of late I have experienced some frustrations. I would like to update my accounts from an iPad rather than on a full desktop PC, yet there are no good GnuCash iPad applications that I could find, and the closed-garden model of Apple seems to discourage any official port to iOS. Also, from my experience with using other accounts packages, I like the search bar function that filters the transactions in real-time allowing you to focus in on a specific category or type of transaction. The search facilities in GnuCash feel out-dated in comparison, infact the user interface in general is starting to feel a little dated.

With these in mind I thought I would try some other personal finance package to see if they meet my requirements, but was surprised to find a lack of export options, particular for the de-facto QIF standard. This utility fills that gap.

For those who would prefer a Java version of this utility, I've also written one in another repo: https://github.com/Jason-Carter/jGnuCash2Qif

# Build Prerequesites

To build GnuCash2Qif you will require the following installed on your system:

 * Microsoft .Net Core 2.1.0

This should be available from Microsoft for Windows, OS X and Linux systems.


# Build Instructions
GnuCash2Qif can be build in Visual Studio, or by following these command line instructions:

```
$ git clone https://github.com/Jason-Carter/GnuCash2Qif.git
$ cd GnuCash2Qif
$ dotnet build GnuCash2Qif.sln
```

If you want to package this into a single executable, you can use a utility called dotnet-warp which is installable as follows:

```
$ dotnet tool install -g dotnet-warp
```

(I've followed Scott Hanselman's instrucntions for this from his following blog entry: https://www.hanselman.com/blog/BrainstormingCreatingASmallSingleSelfcontainedExecutableOutOfANETCoreApplication.aspx)

Once installed, from the .\GnuCashSql2Qif console project run dotnet-warp:

```
$ dotnet-warp
```

This will create a GnuCashSql2Qif.exe under the GnuCashSql2Qif project folder.

# Prebuild Download
In the releases area of this repository you can download a single executable:

https://github.com/Jason-Carter/GnuCash2Qif/releases

This has been built using the build instructions above and contains all of the associated libraries in one executable.

# Usage

```
$ dotnet GnuCashSql2Qif.dll -d "C:/accounts.sql.gnucash.sqlite" -o "C:/output.qif"
```
# Options

 * `-d` Datafile for GnuCash: Must be an SQL Lite datafile (does not work with the XML files)
 * `-o` Output File which will be in QIF format
