using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Service.Common.Model
{
    public class ResponseModel<T>
    {
        private Result _result = ErrorResponse.General.InternalError;

        public ResponseModel(string requestRefNo)
        {
            Payload = typeof(T) == typeof(string) ? default(T) : (T)RuntimeHelpers.GetUninitializedObject(typeof(T));
            RequestRefNo = requestRefNo;
            ResponseRefNo = Guid.NewGuid().ToString();
            IsSuccess = false;
            Code = _result.Code;
            Description = _result.Description;
            Message = _result.Message;
        }
        public T Payload { get; set; }
        public string ResponseRefNo { get; private set; }
        public string RequestRefNo { get; private set; }
        public bool IsSuccess { get; private set; }
        public string Code { get; private set; }
        public string Description { get; private set; }
        public string Message { get; private set; }
        public bool IsSpecialError { get; private set; }

        public void Succeeded()
        {
            _result = ErrorResponse.General.Sucess;
            IsSuccess = true;
            Code = _result.Code;
            Description = _result.Description;
            Message = _result.Message;
        }


        public void Succeeded(T payload)
        {
            _result = ErrorResponse.General.Sucess;
            Payload = payload;
            IsSuccess = true;
            Code = _result.Code;
            Description = _result.Description;
            Message = _result.Message;
        }


        public void Succeeded<A>(T payload, Action<A> action = null, A param = default)
        {
            _result = ErrorResponse.General.Sucess;
            Payload = payload;
            IsSuccess = true;
            Code = _result.Code;
            Description = _result.Description;
            Message = _result.Message;
            if (action != null)
            {
                action(param);
            }
        }

        public void Failed(Result boxResult)
        {
            _result = boxResult;
            IsSuccess = false;
            Code = boxResult?.Code;
            Description = boxResult?.Description;
            Message = boxResult?.Message;
            IsSpecialError = boxResult?.IsSpecialError ?? false;
        }



        public R Failed<A, R>(Result boxResult, Func<A, R> action = null, A param = default)
        {
            _result = boxResult;
            IsSuccess = false;
            Code = boxResult.Code;
            Description = boxResult.Description;
            Message = boxResult.Message;
            if (action != null)
            {
                return action(param);
            }
            return default;
        }


        public void Failed(string description, string message)
        {
            _result = ErrorResponse.General.InternalError;
            IsSuccess = false;
            Description = description;
            Message = message;
        }

        public Result GetResult()
        {
            return _result;
        }
    }
}
