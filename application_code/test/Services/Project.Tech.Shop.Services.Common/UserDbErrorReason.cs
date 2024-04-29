using System.ComponentModel;

namespace block.chain.services.Common;

public enum UserDbErrorReason
{
    [Description("Not found in database")]
    NotFound,
    [Description("No unique")]
    NotUnique,
    [Description("General error")]
    General
}