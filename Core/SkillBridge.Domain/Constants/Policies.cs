using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Domain.Constants;

public static class Policies
{

    public const string ManageProfile = "ManageProfile";
    public const string AdminOnly = "AdminOnly";
    public const string MentorOrAdmin = "MentorOrAdmin";
}
