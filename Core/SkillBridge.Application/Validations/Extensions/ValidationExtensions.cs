using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Validations.Extensions;

public static class ValidationExtensions
{
    public static IRuleBuilderOptions<T, string?> ValidLink<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Must(link =>
            {
                if (string.IsNullOrWhiteSpace(link)) return true;
                return Uri.TryCreate(link, UriKind.Absolute, out var outUri)
                       && (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps);
            });
    }

    public static IRuleBuilderOptions<T, string?> LinkedInUrl<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .ValidLink().WithMessage("Please enter a valid URL.")
            .Must(url => string.IsNullOrEmpty(url) || url.Contains("linkedin.com"))
            .WithMessage("Please enter a valid LinkedIn profile URL.");
    }
}