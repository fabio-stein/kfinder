using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public static class TickUtil
    {
        public static readonly int tickSpeed = 50;
        private static float totalTime = 0;
        private static int lastTickTime = -1;

        public static bool Tick()
        {
            totalTime += Time.deltaTime * tickSpeed;
            var currentTotalTime = (int)totalTime;
            var newTick = currentTotalTime > lastTickTime;
            if (newTick)
                lastTickTime = currentTotalTime;
            return newTick;
        }
    }
}
