using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace Dashboard.Core.DBTransaction
{
    public class DBTransactionExtension
    {
        public static bool Excute(out string errorMsg, params Action[] actions)
        {
            //使用ReadCommitted隔离级别，保持与Sql Server的默认隔离级别一致
            return Excute(out errorMsg, IsolationLevel.ReadCommitted, null, actions);

        }

        public static void Excute(out string errorMsg, IsolationLevel level, params Action[] actions)
        {
            Excute(out errorMsg, level, null, actions);
        }

        public static void Excute(out string errorMsg, int timeOut, params Action[] actions)
        {
            Excute(out errorMsg, IsolationLevel.ReadCommitted, timeOut, actions);
        }

        public static bool Excute(out string errorMsg,  IsolationLevel level, int? timeOut, params Action[] actions)
        {
            errorMsg = "";
            if (actions == null || actions.Length == 0)
                return false;
            TransactionOptions options = new TransactionOptions();

            options.IsolationLevel = level; //默认为Serializable,这里根据参数来进行调整

            if (timeOut.HasValue)

                options.Timeout = new TimeSpan(0, 0, timeOut.Value); //默认60秒

            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    Array.ForEach<Action>(actions, action => action());
                    tran.Complete(); //通知事务管理器它可以提交事务
                    return true;
                }
                catch (Exception ex)
                {
                    errorMsg = ex.Message;
                    return false;
                }
            }
        }
    }
}
