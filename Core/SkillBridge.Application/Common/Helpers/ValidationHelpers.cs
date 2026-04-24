using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Common.Helpers;

public static class ValidationHelpers
{ 
    public static bool LinkMustBeValid(string? link)
    {
        if (string.IsNullOrWhiteSpace(link)) return true; 

        return Uri.TryCreate(link, UriKind.Absolute, out var outUri)
               && (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps);
    }
}