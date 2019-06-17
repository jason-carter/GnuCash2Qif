
namespace GnuCash.Sql2Qif.Library.BLL
{
    public interface IAccountSplit
    {
        IAccount Account { get; set; }
        decimal Trxvalue { get; set; }
    }
}
