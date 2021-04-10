using System.Collections;
using _Src.Scripts;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    public float destroyTime;
    public Rigidbody rb;
    public float speed = 200f;

    public AudioClip hitSound;
    public ParticleSystem hitParticle;

    private IEnumerator DelayedDisable()
    {
        yield return new WaitForSeconds(destroyTime);
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        rb.velocity = transform.forward * speed;
        StartCoroutine(DelayedDisable());
    }

    private void OnDisable()
    {
        GetComponentInParent<Ammo>().PushBullet(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamagable>(out var damagable))
        {
            damagable.TakeDamage();
        }

        AudioSource.PlayClipAtPoint(hitSound, transform.position);

        rb.Sleep();
        hitParticle.Play();
        transform.GetChild(0).gameObject.SetActive(false);
    }
}