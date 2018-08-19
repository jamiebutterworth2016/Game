using System.Runtime.Serialization;

namespace Game.Items.Shields
{
    [DataContract]
    [KnownType(typeof(Buckler))]
    public abstract class Shield
    {
        public abstract decimal ChanceToBlock { get; }

        public bool HasBlocked()
        {
            var random = RandomNumberGenerator.GetRandomDecimal(0m, 100m);

            if (ChanceToBlock <= random)
                return true;

            return false;
        }
    }
}