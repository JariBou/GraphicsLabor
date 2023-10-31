using System;
using UnityEngine;

namespace GraphicsLabor.Scripts.Editor.Utility
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Returns true if self inherits. Can be used to know if an object can be casted or downCasted to a certain Type
        /// </summary>
        /// <param name="self">The object the method is called on</param>
        /// <param name="parentType">The type to test against</param>
        /// <param name="includeSelfType">If set to false and parentType==self.GetType() return false</param>
        /// <returns></returns>
        public static bool InheritsFrom(this object self, Type parentType, bool includeSelfType = true)
        {
            if (!includeSelfType && self.GetType() == parentType) return false;
            
            return ReflectionUtility.GetSelfAndBaseTypes(self).Contains(parentType);
        }
    }

    public static class QuaternionExtensions
    {
        /// <summary>
        /// Returns a Quaternion as a Vector4
        /// </summary>
        /// <param name="quaternion">The quaternion the method is called on</param>
        /// <returns></returns>
        public static Vector4 AsVector4(this Quaternion quaternion)
        {
            return new Vector4(quaternion.x, quaternion.y, quaternion.w, quaternion.z);
        }
    }
}