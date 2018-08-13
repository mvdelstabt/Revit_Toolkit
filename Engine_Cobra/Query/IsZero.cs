﻿using System;

namespace BH.UI.Cobra.Engine
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        
        static public bool IsZero(double @double)
        {
            return oM.Geometry.Tolerance.MicroDistance > Math.Abs(@double);
        }

        /***************************************************/
    }
}