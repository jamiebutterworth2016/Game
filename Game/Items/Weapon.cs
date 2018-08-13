﻿using System.Runtime.Serialization;

namespace Game.Items
{
    [DataContract]
    [KnownType(typeof(Sword))]
    public abstract class Weapon
    {
        public abstract decimal MinDamage { get; }

        public abstract decimal MaxDamage { get; }
    }
}