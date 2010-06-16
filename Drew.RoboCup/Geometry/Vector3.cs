﻿/*
 * Created by Drew, 07/05/2010 02:50.
 */
using System;

namespace Drew.RoboCup
{
	public struct Vector3
	{
	    public static readonly Vector3 Origin = new Vector3(0,0,0);
	    
	    public double X { get; private set; }
	    public double Y { get; private set; }
	    public double Z { get; private set; }

	    public Vector3(double x, double y, double z) : this() {
			X = x;
			Y = y;
			Z = z;
		}
	    
	    public static Vector3 operator -(Vector3 a, Vector3 b) {
	            return new Vector3(
	                a.X - b.X,
	                a.Y - b.Y,
	                a.Z - b.Z);
	    }
	
	    public static Vector3 operator +(Vector3 a, Vector3 b) {
	            return new Vector3(
	                a.X + b.X,
	                a.Y + b.Y,
	                a.Z + b.Z);
	    }
	
	    public Vector3 Normalize() {
            // norm2 = sqrt(xax[0] * xax[0] + xax[1] * xax[1] + xax[2] * xax[2]);
            // for(i=0; i<3; i++) xax[i] /= norm2;
            var length = GetLength();
            
            // avoid DivideByZeroException
            if (length==0)
                return new Vector3();
            
            return new Vector3(X/length, Y/length, Z/length);
	    }
	    
	    public double GetLength() {
	         return Math.Sqrt(X * X + Y * Y + Z * Z);
	    }
	
		public override string ToString() {
		    return string.Format("<{0:0.00},{1:0.00},{2:0.00}>", X, Y, Z);
		}
	    
        public override bool Equals(object obj) {
	        if (!(obj is Vector3))
	            return false;
	        Vector3 that = (Vector3)obj;
	        return Math.Abs(this.X - that.X) < 0.00001 &&
	               Math.Abs(this.Y - that.Y) < 0.00001 &&
	               Math.Abs(this.Z - that.Z) < 0.00001;
        }
	    
        public override int GetHashCode() {
	        unchecked {
	            int c = (int)X;
	            c += (int)(Y * 5);
	            c += (int)(Z * 13);
	            return c;
	        }
        }
	}
}