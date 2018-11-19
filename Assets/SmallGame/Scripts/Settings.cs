using Unity.Mathematics;
using UnityEngine;

namespace SmallGame
{
    [CreateAssetMenu]
    public class Settings : ScriptableObject
    {
        public float2 randomDistance = new float2(2f,3f);
        public float2 randomSize = new float2(1f,1.8f);
        public float maxJumperHoldTime = 1.5f;
        public float jumpTime = .5f;
        public float jumpSpeed = 15f;
    }
}
