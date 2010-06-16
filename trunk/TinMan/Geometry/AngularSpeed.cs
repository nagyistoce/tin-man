﻿/*
 * Created by Drew, 10/06/2010 04:37.
 */
using System;

namespace TinMan
{
    /// <summary>
    /// Represents an angular speed as a double-precision floating point value.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{DegreesPerSecond} deg/sec")]
    public struct AngularSpeed : IEquatable<AngularSpeed>
    {
        public static readonly AngularSpeed Zero = new AngularSpeed(0);
        public static readonly AngularSpeed NaN = new AngularSpeed(double.NaN);
        
        #region Static factory methods and private constructor

        public static AngularSpeed FromRadiansPerSecond(double degreesPerSecond) {
            return new AngularSpeed(degreesPerSecond);
        }
        
        public static AngularSpeed FromDegreesPerSecond(double degreesPerSecond) {
            return new AngularSpeed(Angle.DegreesToRadians(degreesPerSecond));
        }
        
        private AngularSpeed(double radiansPerSecond) : this() {
            RadiansPerSecond = radiansPerSecond;
        }
        
        #endregion

        #region Properties
        
        public double RadiansPerSecond { get; private set; }
        public double DegreesPerSecond {
            get { return Angle.RadiansToDegrees(RadiansPerSecond); }
        }
        
        public bool IsNaN { get { return double.IsNaN(RadiansPerSecond); } }
        public AngularSpeed Abs { get { return new AngularSpeed(Math.Abs(RadiansPerSecond)); } }
        
        #endregion
        
        public AngularSpeed Limit(AngularSpeed lowerLimit, AngularSpeed upperLimit) {
            if (lowerLimit > upperLimit)
                throw new ArgumentException("The lower limit must be less than the upper limit.");
            if (this < lowerLimit)
                return lowerLimit;
            if (this > upperLimit)
                return upperLimit;
            return this;
        }

        #region Operators, Equality and Hashing
        
        public override bool Equals(object obj) {
            return (obj is AngularSpeed) ? Equals((AngularSpeed)obj) : false;
        }
        
        public bool Equals(AngularSpeed other) {
            return other.RadiansPerSecond == RadiansPerSecond;
        }
        
        public override int GetHashCode() {
            return RadiansPerSecond.GetHashCode();
        }
        
        public static AngularSpeed operator +(AngularSpeed a, AngularSpeed b) {
            return AngularSpeed.FromRadiansPerSecond(a.RadiansPerSecond + b.RadiansPerSecond);
        }
        
        public static AngularSpeed operator -(AngularSpeed a, AngularSpeed b) {
            return AngularSpeed.FromRadiansPerSecond(a.RadiansPerSecond - b.RadiansPerSecond);
        }
        
        public static AngularSpeed operator -(AngularSpeed a) {
            return AngularSpeed.FromRadiansPerSecond(-a.RadiansPerSecond);
        }
        
        public static AngularSpeed operator *(AngularSpeed a, double scale) {
            return AngularSpeed.FromRadiansPerSecond(a.RadiansPerSecond * scale);
        }
        
        public static AngularSpeed operator /(AngularSpeed a, double quotient) {
            return AngularSpeed.FromRadiansPerSecond(a.RadiansPerSecond / quotient);
        }

        public static Angle operator *(AngularSpeed a, TimeSpan time) {
            return Angle.FromRadians(a.RadiansPerSecond * time.TotalSeconds);
        }

        public static bool operator >(AngularSpeed left, AngularSpeed right) {
            return left.RadiansPerSecond > right.RadiansPerSecond;
        }

        public static bool operator <(AngularSpeed left, AngularSpeed right) {
            return left.RadiansPerSecond < right.RadiansPerSecond;
        }

        public static bool operator >=(AngularSpeed left, AngularSpeed right) {
            return left.RadiansPerSecond >= right.RadiansPerSecond;
        }

        public static bool operator <=(AngularSpeed left, AngularSpeed right) {
            return left.RadiansPerSecond <= right.RadiansPerSecond;
        }

        public static bool operator ==(AngularSpeed left, AngularSpeed right) {
            return left.Equals(right);
        }
        
        public static bool operator !=(AngularSpeed left, AngularSpeed right) {
            return !left.Equals(right);
        }
        #endregion
    }
}
