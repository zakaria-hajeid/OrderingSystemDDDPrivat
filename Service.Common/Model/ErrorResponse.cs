using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Common.Model
{
    public static class ErrorResponse
    {
        public static class General
        {
            public static Result Sucess => new Result("Sucess", "تم تنفيذ العملية بنجاح", "S000");
            public static Result InternalError => new Result("InternalError", "حدث خطأ أثناء تنفيذ الطلب", "E001");
            public static Result InvalidPermissions => new Result("InvalidPermissions", "عفوا ، ليس لديك صلاحية لاستخدام الخدمة", "E002");
            public static Result SpecificMessage(string error) => new Result("SpecificMessage", error, "E002");

        }
    }
}
