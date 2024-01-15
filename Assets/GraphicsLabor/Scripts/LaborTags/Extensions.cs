using GraphicsLabor.Scripts.Core.Utility;
using UnityEngine;

namespace GraphicsLabor.Scripts.LaborTags
{
    public static class Extensions
    {

        public static bool HasExactTags(this MonoBehaviour self, LaborTags tags)
        {
            ITagHolder component = self.GetComponent<ITagHolder>();
            if (component == null)
            {
                throw new MissingComponentException($"{nameof(self)} is missing required ITagHolder component");
            }

            return component.GetLaborTags() == tags;
        }

        //TODO doc here
        public static bool HasTags(this MonoBehaviour self, LaborTags tags, bool allowNothing = false)
        {
            if (!allowNothing && (int)tags == 0) return false;
            
            ITagHolder component = self.GetComponent<ITagHolder>();
            if (component == null)
            {
                throw new MissingComponentException($"{nameof(self)} is missing required ITagHolder component");
            } 

            int result = (int)component.GetLaborTags() & (int)tags;

            return result == (int)tags;
        }

    }
}