﻿using System.ComponentModel;

public enum PeriodType
{
    [Description("day")]
    day,
    [Description("month")]
    month,
    [Description("year")]
    year,
    [Description("ytd")]
    ytd
}