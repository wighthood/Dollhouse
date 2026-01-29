using System;
using UnityEngine;

    
    public class ColliderDetector : MonoBehaviour
    {
        internal Action<Collider2D> _onTriggerEnterFunction= x => { return; } ;
        internal Action<Collider2D> _onTriggerExitFunction= x => { return; } ;
        private void OnTriggerEnter2D(Collider2D other)
        {
            _onTriggerEnterFunction?.Invoke(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _onTriggerExitFunction?.Invoke(other);
        }
    }