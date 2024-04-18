using System.ComponentModel;

namespace Project.Tech.Shop.Services.Common;

public enum UserDbErrorReason
{
    [Description("Not found in database")]
    NotFound,
    [Description("No unique")]
    NotUnique,
    [Description("General error")]
    General
}