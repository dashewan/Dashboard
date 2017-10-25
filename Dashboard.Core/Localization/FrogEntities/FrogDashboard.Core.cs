using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dashboard.Core.FrogEntities
{
    public abstract class DBBase : IDisposable
    //where TEntity : class
    {
        #region Fields and Constructors
        private DashboardDbContext _dbc;

        /// <summary>
        /// RyanDing.
        /// 
        /// Pass an existing datacontext and reuse it.
        /// </summary>
        protected DBBase(DashboardDbContext dbc)
        {
            this._dbc = dbc;
        }

        /// <summary>
        /// RyanDing.
        /// 
        /// Default use a new context.
        /// </summary>
        protected DBBase()
            : this(CreateDataContext())
        {
        }



        #endregion

        #region Private Methods
        private static DashboardDbContext CreateDataContext()
        {
            var dbc = new DashboardDbContext();
            return dbc;
        }
        #endregion

        #region Public Properties
        public DashboardDbContext DataContext
        {
            get { return _dbc; }
            protected set { _dbc = value; }
        }
        #endregion

        #region Public Methods
        public void Dispose()
        {
            if (DataContext != null)
            {
                DataContext.Dispose();
                DataContext = null;
            }
        }

        /// <summary>
        /// 公用分页类
        /// </summary>
        /// <typeparam name="T">数据分页</typeparam>
        /// <param name="DataSource">分页的数据源</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="pageIndex">每一页的起始值</param>
        /// <param name="pageCount">分页后的页数</param>  
        /// <returns>分页后的结果集</returns>
        public static IQueryable<T> Paging<T>(IQueryable<T> DataSource, int pageSize, int pageIndex, out int pageCount)
        {
            int totalRecordCount = DataSource.Count();
            int totalPageCount = 0;

            pageSize = pageSize == 0 ? totalRecordCount : pageSize;

            totalPageCount = totalRecordCount % pageSize == 0 ? totalRecordCount / pageSize : totalRecordCount / pageSize + 1;
            pageCount = totalPageCount;

            IQueryable<T> result = DataSource.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            return result;

        }

        #endregion

        #region Public abstract Methods
        //public abstract TEntity GetEntity(int id);
        #endregion
    }
}
