using NodeSystem.Runtime.References;
using UnityEngine;

namespace NodeSystem.Runtime.Extensions
{
    public static class GameObjectExtensions
    {
        public static Component GetReferencedComponent(this GameObject self, string guid)
        {
            GameObjectComponentReferenceBank bank = self.GetComponent<GameObjectComponentReferenceBank>();
            return bank == null ? null : bank.GetComp<Component>(guid);
        }

        public static T GetReferencedComponent<T>(this GameObject self, string guid) where T : Component
        {
            GameObjectComponentReferenceBank bank = self.GetComponent<GameObjectComponentReferenceBank>();
            return bank == null ? null : bank.GetComp<T>(guid);
        }
    }
}