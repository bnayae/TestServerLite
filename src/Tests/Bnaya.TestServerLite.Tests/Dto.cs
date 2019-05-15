using System;
using System.Collections.Generic;
using System.Text;

namespace Bnaya.TestServerLite.Tests
{
    public class Dto : IEquatable<Dto>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Dto);
        }

        public bool Equals(Dto other)
        {
            return other != null &&
                   Id == other.Id &&
                   Name == other.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name);
        }

        public static bool operator ==(Dto left, Dto right)
        {
            return EqualityComparer<Dto>.Default.Equals(left, right);
        }

        public static bool operator !=(Dto left, Dto right)
        {
            return !(left == right);
        }
    }
}
