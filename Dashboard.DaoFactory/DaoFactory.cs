using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dashboard.DaoFactory
{
    public class ObjectFactory
    {
        #region ApplicationContext
        /// <summary>
        /// ApplicationContext
        /// </summary>
        private static Spring.Context.IApplicationContext _ApplicationContext;
        private static Spring.Context.IApplicationContext ApplicationContext
        {
            get
            {
                try
                {
                    if (_ApplicationContext == null)
                    {
                        _ApplicationContext = Spring.Context.Support.ContextRegistry.GetContext();
                    }
                    return _ApplicationContext;
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                    return null;
                }
            }
        }
        #endregion

        #region getObjectByConfigCode
        /// <summary>
        /// getObjectByConfigCode
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="code"></param>
        /// <returns></returns>
        public static T getObjectByConfigCode<T>(string code) //where T : AppServiceInterface.IServcie //Interface.IDaoService
        {
            try
            {
                T objInterface = (T)ApplicationContext.GetObject(code);
                return objInterface;
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }
        #endregion
    }
}
