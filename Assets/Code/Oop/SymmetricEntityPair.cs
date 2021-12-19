using System;
using Kk.BusyEcs;

namespace Code.Oop
{
    internal readonly struct SymmetricEntityPair : IEquatable<SymmetricEntityPair>
    {
        private readonly int _a;
        private readonly int _b;

        public SymmetricEntityPair(Entity a, Entity b)
        {
            _a = a.GetInternalId();
            _b = b.GetInternalId();
            if (_a > _b)
            {
                (_a, _b) = (_b, _a);
            }
        }

        public bool Equals(SymmetricEntityPair other)
        {
            return _a == other._a && _b == other._b;
        }

        public override bool Equals(object obj)
        {
            return obj is SymmetricEntityPair other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_a * 397) ^ _b;
            }
        }
    }
}