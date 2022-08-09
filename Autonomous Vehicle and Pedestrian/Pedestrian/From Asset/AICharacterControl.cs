using System;
using UnityEngine;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (UnityEngine.AI.NavMeshAgent))]
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class AICharacterControl : MonoBehaviour
    {
        public UnityEngine.AI.NavMeshAgent agent { get; private set; }             // the navmesh agent required for the path finding
        public ThirdPersonCharacter character { get; private set; } // the character we are controlling
        public Vector3 target;                                    // target to aim for
        public WaypointNavigator WPN;

        private void Start()
        {
            // get the components on the object we need ( should not be null due to require component so no need to check )
            agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
            character = GetComponent<ThirdPersonCharacter>();
            WPN = GetComponent<WaypointNavigator>();

	        agent.updateRotation = false;
	        agent.updatePosition = true;
        }


        private void Update()
        {   
            UpdateSpeed();
            if (target != null){
                agent.SetDestination(target);
            }
            if (agent.remainingDistance > agent.stoppingDistance)
            {
                character.Move(agent.desiredVelocity, false, false);
            }
            if(agent.remainingDistance<=agent.stoppingDistance)
            {
                WPN.UpdatePosition();
                agent.SetDestination(target);
            }
            else
            {
                character.Move(Vector3.zero, false, false);
            }
        }


        public void SetTarget(in Vector3 a_target)
        {
            target = a_target;
        }

        private void UpdateSpeed()
        {
            float speed =agent.speed;
            int var=Mathf.RoundToInt(UnityEngine.Random.Range(0f,10.5f));
            switch (var)
            {
                case 10:
                    agent.speed=(speed+0.1f<1.5f)?speed+0.1f:1.5f;
                    break;
                case 9:
                    agent.speed=(speed-0.1f>0.3f)? speed-0.1f:0.3f;
                    break;
                default:
                    break; 
            }
        }

    }
}
