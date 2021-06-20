using System;
using UnityEngine;

namespace _Src.Scripts
{
    [Serializable]
    public class Stats
    {
        [SerializeField] private float _health;
        public float Health
        {
            get => _health;
            set => _health = value > 0 ? value : 0;
        }
    }
}