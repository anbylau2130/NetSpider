﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKick.HttpLib.Collector
{
    public static class DisableCollector
    {
        public static void Disable()
        {
            BaseCollector.CollectStats = false;
        }
    }
}
