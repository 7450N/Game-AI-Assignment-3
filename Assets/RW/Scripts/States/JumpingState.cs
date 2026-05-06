/*
 * Copyright (c) 2019 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using UnityEngine;

namespace RayWenderlich.Unity.StatePatternInUnity
{
    public class JumpingState : State
    {
        private bool grounded;
        private int jumpParam = Animator.StringToHash("Jump");
        private int landParam = Animator.StringToHash("Land");
        private bool dive;
        private bool finishedDiving;

        public JumpingState(Character character, StateMachine stateMachine) : base(character, stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            SoundManager.Instance.PlaySound(SoundManager.Instance.jumpSounds);
            grounded = false;
            dive = false;
            finishedDiving = true;
            Jump();
        }

        public override void HandleInput()
        {
            base.HandleInput();
            if ((Input.GetKeyDown(KeyCode.S)) && (character.transform.position.y > character.DiveThreshold) )
            {
                dive = true;
                finishedDiving = false;
            }
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (!finishedDiving)
            {
                Dive();
                finishedDiving = true;
                if (grounded)
                {
                    character.DiveBomb();
                }
            }
            if (grounded)
            {   
                if (dive)
                {
                    character.DiveBomb();
                    stateMachine.ChangeState(character.standing);
                }
                else
                {
                    character.TriggerAnimation(landParam);
                    SoundManager.Instance.PlaySound(SoundManager.Instance.landing);
                    stateMachine.ChangeState(character.standing);
                }
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            grounded = character.CheckCollisionOverlap(character.transform.position);
        }

        private void Jump()
        {
            character.transform.Translate(Vector3.up * (character.CollisionOverlapRadius + 0.1f));
            character.ApplyImpulse(Vector3.up * character.JumpForce);
            character.TriggerAnimation(jumpParam);
        }

        private void Dive()
        {
            character.transform.Translate(Vector3.down * (character.CollisionOverlapRadius + 0.1f));
            character.ApplyImpulse(Vector3.down * character.DiveForce);
        }
    }
}
