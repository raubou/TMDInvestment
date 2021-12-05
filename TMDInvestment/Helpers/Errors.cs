﻿using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace TMDInvestment.Helpers
{
    public class Errors
    {
        public static bool HasErrors(dynamic results)
        {
            bool hasErrors;
            try
            {
                if(results.error != null)
                {
                    hasErrors = true;
                }
                else
                {
                    hasErrors = false;
                }
            }
            catch (Exception ex)
            {
                try
                {
                    if (!string.IsNullOrEmpty(((JsonElement)results).GetProperty("error").GetString()))
                    {
                        hasErrors = true;
                    }
                    else
                    {
                        hasErrors = false;
                    }
                }
                catch (Exception ex2)
                {
                    hasErrors = false;
                }
            }
            return hasErrors;
        }

        public static bool NotFound(dynamic results)
        {
            bool hasErrors;
            try
            {
                if (results.message != null && results.message == "NotFound")
                {
                    hasErrors = true;
                }
                else
                {
                    hasErrors = false;
                }
            }
            catch (Exception ex)
            {
                //no errors
                hasErrors = false;
            }
            return hasErrors;
        }

    }
}
