using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectNadir
{
    public abstract class State
    {

        protected PlayerMovement playerMovement;

        //constructor
        public State(PlayerMovement inPlayerMovement)
        {
            playerMovement = inPlayerMovement;
        }


        #region StateBehaviors
        public virtual IEnumerator Start()
        {
            yield break;
        }
        public virtual IEnumerator Walk()
        {
            yield break;
        }
        public virtual IEnumerator Jump()
        {
            yield break;
        }
        public virtual IEnumerator Dash() 
        {
            yield break;
        }
        public virtual IEnumerator Attack() 
        {
            yield break;
        }


        public virtual void StateManager()
        {
            return;
        }
        public virtual void ApplyGravity() 
        {
            return;
        }

        #endregion

        #region Methods
        protected void UpdateMovedirection(float Speed)
        {
            playerMovement.moveDirection.x = Speed * Time.fixedDeltaTime * playerMovement.inputDirection.x;
            playerMovement.moveDirection.z = Speed * Time.fixedDeltaTime * playerMovement.inputDirection.y;
        }
        protected void ApplyJumpForce(float inFloat)
        {
            playerMovement.moveDirection.y = inFloat;
        }
        protected void StartDash() 
        {
            if (playerMovement.IsGrounded == false)
            {
                playerMovement.moveDirection.y = 0f;
            }
            playerMovement.moveDirection.x = playerMovement.DashSpeed * Time.fixedDeltaTime * Vector3.Normalize(playerMovement.neutralDirection).x;
            playerMovement.moveDirection.z = playerMovement.DashSpeed * Time.fixedDeltaTime * Vector3.Normalize(playerMovement.neutralDirection).z;

        }
        protected void YepGravity() 
        {
            if (playerMovement.IsGrounded == false)
            {
                playerMovement.moveDirection.y -= playerMovement.Gravity * Time.fixedDeltaTime;
            }
        }

        #endregion
    }
}
