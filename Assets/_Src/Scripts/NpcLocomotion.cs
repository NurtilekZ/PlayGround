using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace _Src.Scripts
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NpcLocomotion : MonoBehaviour, IDamagable
    {
        [SerializeField] private NavMeshAgent _navMeshAgent;
        [SerializeField] private Transform _target;
        [SerializeField] private Renderer _renderer;

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _renderer = GetComponent<Renderer>();
        }

        private void Update()
        {
            _navMeshAgent.SetDestination(_target.position);
        }

        public IEnumerator TakeDamage()
        {
            var material = _renderer.material;
            material.color = Color.red;
            yield return new WaitForSeconds(0.01f);
            material.color = Color.white;
        }
    }
}