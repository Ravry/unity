using UnityEngine;

public class AnimatorIKFoot : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] private Transform leftKnee, rightKnee;
    [SerializeField] private LayerMask groundLayerMask;

    private RaycastHit hitInfo_L, hitInfo_R;

    void OnAnimatorIK(int layerIndex)
    {
        Physics.Raycast(leftKnee.position, Vector3.down, out hitInfo_L, 10.0f, groundLayerMask);
        Physics.Raycast(rightKnee.position, Vector3.down, out hitInfo_R, 10.0f, groundLayerMask);

        // animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
        // animator.SetIKPosition(AvatarIKGoal.LeftFoot, hitInfo_L.point);
        
        // animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
        // animator.SetIKPosition(AvatarIKGoal.RightFoot, hitInfo_R.point);
    }

    void OnDrawGizmos() 
    {
        // if (hitInfo_L == null || hitInfo_R == null)
        //     return;

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(hitInfo_L.point, .1f);
        Gizmos.DrawWireSphere(hitInfo_R.point, .1f);
    }
}
