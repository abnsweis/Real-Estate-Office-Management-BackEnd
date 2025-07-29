using FluentResults;

namespace RealEstate.Application.Dtos.ResponseDTO
{

    public class AppResponse
    {
        public Result Result { get; set; }
        public object? Data { get; set; }

        public AppResponse(Result result, object? data)
        {
            Result = result;
            Data = data;
        }

        public AppResponse()
        {
          
        }
        public static AppResponse Success(object? data = null)
        {
            return new AppResponse
            {
                Result = Result.Ok(),
                Data = data
            };
        }

        public static AppResponse Fail(List<IError> errors)
        {
            return new AppResponse
            {
                Result = Result.Fail(errors),
                Data = null
            };
        }
        public static AppResponse Fail(IError error)
        {
            return new AppResponse
            {
                Result = Result.Fail(error),
                Data = null
            };
        }

    }



    public class AppResponse<T>
    {
        public Result Result { get; set; }
        public T? Data { get; set; }

        public static AppResponse<T> Success(T data, Dictionary<string, object>? meta = null)
        {
            return new AppResponse<T>
            {
                Result = Result.Ok(),
                Data = data,
            };
        }

        public static AppResponse<T> Fail(List<IError> errors)
        {
            return new AppResponse<T>
            {
                Result = Result.Fail(errors),
                Data = default
            };
        }

        public static AppResponse<T> Fail(IError error)
        {
            return new AppResponse<T>
            {
                Result = Result.Fail(error),
                Data = default
            };
        }
    }

}
