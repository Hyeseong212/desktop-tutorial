using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowState : StateMachineBehaviour
{
    Transform enemyTransform;

    Skeleton skeleton;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        skeleton = animator.GetComponent<Skeleton>();
        enemyTransform = animator.GetComponent<Transform>();
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Vector2.Distance(skeleton.player.position, enemyTransform.position) > 1.5f)
            enemyTransform.position = Vector2.MoveTowards(enemyTransform.position, skeleton.player.position, Time.deltaTime * skeleton.speed);
        else
        {
            animator.SetBool("IsFollow", false);
        }
        skeleton.DirectionEnemy(skeleton.player.position.x, enemyTransform.position.x);
    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
