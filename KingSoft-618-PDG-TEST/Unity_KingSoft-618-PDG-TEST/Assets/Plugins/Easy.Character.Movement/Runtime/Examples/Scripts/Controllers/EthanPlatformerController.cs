using ECM.Controllers;
using UnityEngine;

namespace ECM.Examples
{
    /// <summary>
    /// 
    /// Example Character Controller
    /// 
    /// This example shows a tipical platformer controller with double jump eg: maxMidAirJumps = 1
    /// 
    /// </summary>

    public class EthanPlatformerController : BaseCharacterController
    {
        #region METHODS

        private float colliderHeight = 0;
        private float colliderCenterY = 0;
        public override void Awake(){
            base.Awake();
            colliderHeight = GetComponent<CapsuleCollider>().height;
            colliderCenterY = GetComponent<CapsuleCollider>().center.y;
        }
        /// <summary>
        /// Performs Ethan animation.
        /// </summary>
        private bool isValue = false;
        protected override void Animate()
        {
            // If there is no animator, return

            if (animator == null)
                return;

            // Compute move vector in local space

            var move = transform.InverseTransformDirection(moveDirection);

            // Update the animator parameters

            var forwardAmount = move.z;

            animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
            animator.SetFloat("Turn", Mathf.Atan2(move.x, move.z), 0.1f, Time.deltaTime);

            animator.SetBool("OnGround", movement.isGrounded);

            if (!movement.isGrounded)
                animator.SetFloat("Jump", movement.velocity.y, 0.1f, Time.deltaTime);

            if (_midAirJumpCount == 1) {
                animator.SetTrigger("JumpTrigger");
            }
            else {
                animator.ResetTrigger("JumpTrigger");
            }

            if (Input.GetKeyDown(KeyCode.F)) {
                isValue = !isValue;
                animator.SetBool("Crouch", movement.isGrounded && isValue);
                GetComponent<CapsuleCollider>().height = isValue ? colliderHeight / 2 : colliderHeight;
                var y = isValue ? colliderCenterY / 2 : colliderCenterY;
                var center = GetComponent<CapsuleCollider>().center;
                GetComponent<CapsuleCollider>().center = new Vector3(center.x, y, center.z);
            }
            if (Input.GetKey(KeyCode.E)) {
                animator.SetBool("Swim", true);
            }

            if (Input.GetKeyDown(KeyCode.R)) {
                animator.SetBool("Swim", false);
            }
            // Calculate which leg is behind, so as to leave that leg trailing in the jump animation
            // (This code is reliant on the specific run cycle offset in our animations,
            // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)

            var runCycle = Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(0).normalizedTime + 0.2f, 1.0f);
            var jumpLeg = (runCycle < 0.5f ? 1.0f : -1.0f) * forwardAmount;

            if (movement.isGrounded)
                animator.SetFloat("JumpLeg", jumpLeg);
        }

        #endregion
    }
}