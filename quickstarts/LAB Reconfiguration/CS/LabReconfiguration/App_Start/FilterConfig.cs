// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Web;
using System.Web.Mvc;

namespace LabReconfiguration
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}