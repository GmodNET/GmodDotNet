using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmodNET.Math
{
    /// <summary>
    /// Class to represent R^3 vector used in Garry's mod
    /// </summary>
    public class Vector
    {
        //x coordinate
        readonly float x;
        //y coordinate
        readonly float y;
        //z coordinate
        readonly float z;

        /// <summary>
        /// Creates 0 vector.
        /// </summary>
        public Vector()
        {
            x = y = z = 0;
        }
        /// <summary>
        /// Creates vector with corresponding components
        /// </summary>
        /// <param name="x">X component of the vector</param>
        /// <param name="y">Y component of the vector</param>
        /// <param name="z">Z component of the vector</param>
        public Vector(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        /// <summary>
        /// Creates vector with all components equal to val
        /// </summary>
        /// <param name="val">Value for all three components of the vector</param>
        public Vector(float val)
        {
            this.x = this.y = this.z = val;
        }
        /// <summary>
        /// Get X component of the vector
        /// </summary>
        public float X
        {
            get
            {
                return this.x;
            }
        }
        /// <summary>
        /// Get Y component of the vector
        /// </summary>
        public float Y
        {
            get
            {
                return this.y;
            }
        }
        /// <summary>
        /// Get Z component of the vector
        /// </summary>
        public float Z
        {
            get
            {
                return this.z;
            }
        }
    }
}
