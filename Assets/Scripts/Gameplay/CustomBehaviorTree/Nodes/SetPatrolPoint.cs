using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;

namespace MBTExample
{
    [AddComponentMenu("")]
    [MBTNode("Custom Actions/Set Patrol Point")]
    public class SetPatrolPoint : Leaf
    {
        public Vector3Reference variableToSet = new Vector3Reference(VarRefMode.DisableConstant);
        public Transform[] waypoints;
        private int index = 0;
        private int direction = 1;

        public override NodeResult Execute()
        {
            if (waypoints.Length == 0)
            {
                return NodeResult.failure;
            }
            // Ping-pong between waypoints
            if (direction == 1 && index == waypoints.Length - 1)
            {
                direction = -1;
            }
            else if (direction == -1 && index == 0)
            {
                direction = 1;
            }
            index += direction;

            // Set blackboard variable with need waypoint (position)
            variableToSet.Value = waypoints[index].position;
            return NodeResult.success;
        }
    }
}
