using System.Collections.Generic;
using UnityEngine;

namespace _Src.Scripts
{
    public class Ammo : MonoBehaviour
    {
        public int defaultAmmoCount = 30;
        public int ammoCount = 30;
        public Bullet bulletPrefab;
        
        private static Queue<Bullet> ammoQueue = new Queue<Bullet>();

        public void PushBullet(Bullet bullet)
        {
            ammoQueue.Enqueue(bullet);
        }
        
        public Bullet FireBullet()
        {
            ammoCount = ammoCount - 1;
            return ammoQueue.Dequeue();
        }

        private void Awake()
        {
            for (int i = 0; i < ammoCount; i++)
            {
                ammoQueue.Enqueue(Instantiate(bulletPrefab, transform));
            }
        }

        public void Reload()
        {
            ammoCount = defaultAmmoCount;
        }
    }
}