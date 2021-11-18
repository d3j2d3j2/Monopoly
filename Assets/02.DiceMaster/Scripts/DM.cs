// Copyright Michele Pirovano 2014-2015
using UnityEngine;

namespace DiceMaster
{
    /// <summary>
    /// Entry point for static components.
    /// </summary>
    public static class DM
    {
        public static DiceConfig Config
        {
            get { return DiceConfig.Instance; }
        }
    }
}
