using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace _Src.Scripts
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NpcLocomotion : MonoBehaviour, IDamagable
    {
        public NavMeshAgent navMeshAgent;
        public Transform target;
        public Renderer renderer;

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            renderer = GetComponent<Renderer>();
        }

        private void Update()
        {
            navMeshAgent.SetDestination(target.position);
        }

        public IEnumerator TakeDamage()
        {
            var material = renderer.material;
            material.color = Color.red;
            yield return new WaitForSeconds(0.01f);
            material.color = Color.white;
        }
    }
}