using UnityEngine;

namespace GraphicsLabor.Scripts.Editor
{
    public class Utils
    {
        public static Vector4 GetQuaternionAsVector4(Quaternion quaternion)
        {
            return new Vector4(quaternion.x, quaternion.y, quaternion.w, quaternion.z);
        }
    }
}