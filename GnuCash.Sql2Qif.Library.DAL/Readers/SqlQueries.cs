namespace GnuCash.Sql2Qif.Library.DAL.Readers
{
    internal static class SqlQueries
    {
        public const string SqlGetAccounts = @"
            select      acc.guid, acc.name, acc.account_type, acc.description
            from        accounts  acc
            inner join  accounts  p   on p.guid = acc.parent_guid
                                      and p.parent_guid is null
                                      and p.account_type = 'ROOT'
                                      and p.name = 'Root Account'
            where acc.account_type in ('ASSET', 'CREDIT', 'BANK', 'LIABILITY')
            ";

        public const string SqlGetCategories = @"
            select      a.guid, a.name, a.account_type, a.description
            from        accounts a
            where       a.account_type in ('EXPENSE', 'INCOME')
            ";

        public const string SqlGetAccountsAndCategories = SqlGetAccounts + " union all " + SqlGetCategories;

        public const string SqlGetTransactions = @"
            with cteAccounts(guid, name, account_type, description) AS
            (" + SqlGetAccountsAndCategories + @"
            )
            select
                            t.guid              as TrxGuid,
                            acc.guid            as AccGuid,
                            acc.name            as AccountName,
                            t.post_date         as DatePosted,
                            t.Num               as Ref,
                            t.Description,
                            sl.string_val       as Notes,
                            s.reconcile_state   as isReconciled,
                            case acc.account_type
                                when 'EQUITY' then ROUND((s.value_num / -100.0), 2)
                                else ROUND((s.value_num / 100.0), 2)
                            end as trxValue
            from            splits        as s
            inner join      transactions  as t    on t.guid = s.tx_guid
            left outer join cteAccounts   as acc  on acc.guid = s.account_guid
            left outer join slots         as sl   on sl.obj_guid = t.guid and sl.name = 'notes'
            where acc.guid is not null -- ignore transactions with no accounts
            order by        t.guid,
                            t.post_date asc
            ";

        public const string SqlGetAccountsAndCategoryHiearchy = @"
            with recursive cteAccounts(guid, name, account_type, parent_guid, code, description, hidden, placeholder, level, path) AS
            (
                select guid, name, account_type, parent_guid, code,
                        description, hidden, placeholder, 0, ''
                from accounts
                where parent_guid is null
                and name = 'Root Account'
                union all
                select a.guid, a.name, a.account_type, a.parent_guid, a.code,
                            a.description, a.hidden, a.placeholder, p.level + 1, p.path || ':' || a.name
                from        accounts a
                inner join  cteAccounts p on p.guid = a.parent_guid
                order by 9 desc -- by using desc we're doing a depth-first search
            )
            select substr('                                        ', 1, level* 10) || name 'hierarchy',
                    name,
                    guid,
                    description,
                    substr(path, 2, length(path)) 'path',
                    account_type, level
            from    cteAccounts
            where account_type in ('ASSET', 'CREDIT', 'BANK', 'EXPENSE', 'INCOME', 'LIABILITY');
            ";
    }
}
