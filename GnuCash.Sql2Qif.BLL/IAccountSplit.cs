
namespace GnuCash.Sql2Qif.Library.BLL
{
    public interface IAccountSplit
    {
        IAccount Account { get; set; }
        string IsReconciled { get; set; }
        decimal TrxValue { get; set; }
    }
}
