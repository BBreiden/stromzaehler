using System;

namespace Stromzaehler.Models
{
    public enum Source
    {
        None = 0, Power = 1, Water = 2
    }
    public class Blink : IEquatable<Blink>
    {
        public Blink()
        {
        }

        public long BlinkId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public int Value { get; set; }
        public Source Source { get; set; }

        public static bool operator==(Blink a, Blink b) 
            => a is null ? b is null : a.Equals(b);
        public static bool operator!=(Blink a, Blink b) 
            => !(a == b);
        
        public bool Equals(Blink? other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Timestamp == other.Timestamp 
                && Value == other.Value
                && Source == other.Source;
        }

        public override bool Equals(object? obj) 
        {
            return Equals(obj as Blink);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Timestamp, Source, Value);
        }

        public override string ToString() {
            return $"[T={Timestamp.ToString("o")}, C={Value}";
        }
    }
}
