using SkillBridge.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Common.Models;

public class Result : IResult
{
    public bool IsSuccess { get; private set; }
    public string Message { get; private set; }

    protected Result(bool isSuccess, string message)
    {
        IsSuccess = isSuccess;
        Message = message;
    }

    public static Result Success(string message = "Uğurla tamamlandı.")
        => new Result(true, message);

    public static Result Failure(string message)
        => new Result(false, message);
}

public class Result<T> : Result, IResult<T>
{
    public T Data { get; private set; }

    protected Result(T data, bool isSuccess, string message)
        : base(isSuccess, message)
    {
        Data = data;
    }

    public static Result<T> Success(T data, string message = "Uğurla tamamlandı.")
        => new Result<T>(data, true, message);

    public static new Result<T> Failure(string message)
        => new Result<T>(default!, false, message);
}
