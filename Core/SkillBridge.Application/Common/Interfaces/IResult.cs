using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Common.Interfaces;

public interface IResult
{
    bool IsSuccess { get; }
    string Message { get; }
}

public interface IResult<out T> : IResult
{
    T Data { get; }
}
