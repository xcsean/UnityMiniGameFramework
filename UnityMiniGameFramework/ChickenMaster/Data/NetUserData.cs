﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class NetUserData : Data
    {
        public static NetUserData create()
        {
            return new NetUserData();
        }

        public NetUserData()
        {

        }
    }
}
