using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Dashboard.Core.Utility
{
    public class ModelStateExtension
    {
        public static List<string> GetModelError(ViewDataDictionary viewData)
        {
            List<string> listError = new List<string>();
            foreach (ModelState modelState in viewData.ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    listError.Add(error.ErrorMessage);
                }
            }
            return listError;
        }
    }
}
