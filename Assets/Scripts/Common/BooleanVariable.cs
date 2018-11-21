using System;
using UnityEngine;

namespace Scripts.Common
{
    [CreateAssetMenu]
    public class BooleanVariable : UnityEngine.ScriptableObject
    {
        public event Action<bool> ValueChanged;
        private bool _value;

        public bool Value
        {
            get { return _value; }
            set
            {
                _value = value;
                ValueChanged?.Invoke(_value);
            }
        }

        public void Toggle()
        {
            Value = !Value;
        }
    }
}
