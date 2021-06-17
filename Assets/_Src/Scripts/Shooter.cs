using UnityEngine;

namespace _Src.Scripts
{
    public class Shooter : MonoBehaviour
    {
        public Ammo ammo;
        public AudioClip shootSound;
        public float delayTime = 0.5f;
        public bool isReloading;
        public GameObject muzzleFlash;

        public void Shoot(Transform aimTransform)
        {
            AudioSource.PlayClipAtPoint(shootSound, aimTransform.position);
            Bullet bullet = ammo.FireBullet();
            bullet.transform.position = aimTransform.position;
            bullet.transform.rotation = aimTransform.rotation;
            bullet.gameObject.SetActive(true);
        }

        public void Muzzle(bool value)
        {
            muzzleFlash.SetActive(value);
        }

        public void Reload()
        {
            ammo.Reload();
            isReloading = false;
        }
    }
}