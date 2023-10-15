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
        private int _index = 0;
        private int _direction = 1;

        public override NodeResult Execute()
        {
            if (waypoints.Length == 0)
            {
                return NodeResult.failure;
            }
            // Ping-pong between waypoints
            if (_direction == 1 && _index == waypoints.Length - 1)
            {
                _direction = -1;
            }
            else if (_direction == -1 && _index == 0)
            {
                _direction = 1;
            }
            _index += _direction;

            // Set blackboard variable with need waypoint (position)
            variableToSet.Value = waypoints[_index].position;
            return NodeResult.success;
        }
    }
}
