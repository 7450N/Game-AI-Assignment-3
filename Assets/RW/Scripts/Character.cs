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

using System.Collections;
using UnityEngine;

namespace RayWenderlich.Unity.StatePatternInUnity
{
    [RequireComponent(typeof(CapsuleCollider))]
    public class Character : MonoBehaviour
    {
        #region Variables

        public enum EquipmentType
        {
            None,
            Melee,
            Ranged
        }
        private int totalEquipment = 3; // Total number of equipment types

        public StateMachine movementSM;
        public StandingState standing;
        public DuckingState ducking;
        public JumpingState jumping;
        public DieState die;
         

        public StateMachine attackSM;
        public DrawSwordState drawSword;
        public SwingSwordState swingSword;
        public SheathSwordState sheathSword;
        public MagicState magic;
        public AttackIdleState attackIdle; // New state for idle attack

        [HideInInspector] public bool isBlocking = false; // Whether the character is blocking

#pragma warning disable 0649
        [SerializeField]
        private Transform handTransform;
        [SerializeField]
        private Transform sheathTransform;
        [SerializeField]
        private Transform shootTransform;
        [SerializeField]
        private CharacterData data;
        [SerializeField]
        private LayerMask whatIsGround;
        [SerializeField]
        private Collider hitBox;
        [SerializeField]
        private Animator anim;
        [SerializeField]
        private ParticleSystem shockWave;
#pragma warning restore 0649
        [SerializeField]
        private float meleeRestThreshold = 10f;
        [SerializeField]
        private float diveThreshold = 4f;
        [SerializeField]
        private float collisionOverlapRadius = 0.1f;
        [SerializeField] private float playerHP = 100f; // Player's health

        [HideInInspector] public GameObject currentWeapon;

        private int horizonalMoveParam = Animator.StringToHash("H_Speed");
        private int verticalMoveParam = Animator.StringToHash("V_Speed");
        private int shootParam = Animator.StringToHash("Shoot");
        private int hardLanding = Animator.StringToHash("HardLand");

        public EquipmentType currentEquipment = EquipmentType.None;

        private bool attackSMInitialized = false;
        private Sword sword; // Reference to the Sword component

        #endregion

        #region Properties

        public float NormalColliderHeight => data.normalColliderHeight;
        public float CrouchColliderHeight => data.crouchColliderHeight;
        public float DiveForce => data.diveForce;
        public float JumpForce => data.jumpForce;
        public float MovementSpeed => data.movementSpeed;
        public float RunSpeed => data.runSpeed;
        public float CrouchSpeed => data.crouchSpeed;
        public float RotationSpeed => data.rotationSpeed;
        public float CrouchRotationSpeed => data.crouchRotationSpeed;
        public GameObject MeleeWeapon => data.meleeWeapon;
        public GameObject ShootableWeapon => data.staticShootable;
        public float DiveCooldownTimer => data.diveCooldownTimer;
        public float CollisionOverlapRadius => collisionOverlapRadius;
        public float DiveThreshold => diveThreshold;
        public float MeleeRestThreshold => meleeRestThreshold;
        public int isMelee => Animator.StringToHash("IsMelee");
        public int crouchParam => Animator.StringToHash("Crouch");
        public float PlayerHP => playerHP;
        public Animator Animator => anim;

        public float ColliderSize
        {
            get => GetComponent<CapsuleCollider>().height;

            set
            {
                GetComponent<CapsuleCollider>().height = value;
                Vector3 center = GetComponent<CapsuleCollider>().center;
                center.y = value / 2f;
                GetComponent<CapsuleCollider>().center = center;
            }
        }

        #endregion

        #region Methods

        public void Move(float speed, float rotationSpeed)
        {
            Vector3 targetVelocity = speed * transform.forward * Time.deltaTime;
            targetVelocity.y = GetComponent<Rigidbody>().linearVelocity.y;
            GetComponent<Rigidbody>().linearVelocity = targetVelocity;

            GetComponent<Rigidbody>().angularVelocity = rotationSpeed * Vector3.up * Time.deltaTime;

            if (targetVelocity.magnitude > 0.01f || GetComponent<Rigidbody>().angularVelocity.magnitude > 0.01f)
            {
                SoundManager.Instance.PlayFootSteps(Mathf.Abs(speed));
            }

            anim.SetFloat(horizonalMoveParam, GetComponent<Rigidbody>().angularVelocity.y);
            anim.SetFloat(verticalMoveParam, speed * Time.deltaTime);
        }

        public void ResetMoveParams()
        {
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            anim.SetFloat(horizonalMoveParam, 0f);
            anim.SetFloat(verticalMoveParam, 0f);
        }

        public void ApplyImpulse(Vector3 force)
        {
            GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
        }

        public void SetAnimationBool(int param, bool value)
        {
            anim.SetBool(param, value);
        }

        public void TriggerAnimation(int param)
        {
            anim.SetTrigger(param);
        }

        public void Shoot()
        {
            TriggerAnimation(shootParam);
            GameObject shootable = Instantiate(data.shootableObject, shootTransform.position, shootTransform.rotation);
            shootable.GetComponent<Rigidbody>().linearVelocity = shootable.transform.forward * data.bulletInitialSpeed;
            SoundManager.Instance.PlaySound(SoundManager.Instance.shoot, true);
        }

        public bool CheckCollisionOverlap(Vector3 point)
        {
            return Physics.OverlapSphere(point, CollisionOverlapRadius, whatIsGround).Length > 0;
        }

        public void Equip(GameObject weapon = null)
        {
            if (weapon != null)
            {
                currentWeapon = Instantiate(weapon, handTransform.position, handTransform.rotation, handTransform);
            }
            else
            {
                ParentCurrentWeapon(handTransform);
            }
        }


        public void DiveBomb()
        {
            TriggerAnimation(hardLanding);
            float length = anim.GetCurrentAnimatorStateInfo(0).length;
            SoundManager.Instance.PlaySound(SoundManager.Instance.hardLanding);
            shockWave.Play();
        }

        public void SheathWeapon()
        {
            ParentCurrentWeapon(sheathTransform);
        }

        public void Unequip()
        {
            Destroy(currentWeapon);
        }

        public void ActivateHitBox()
        {
            hitBox.enabled = true;
        }

        public void DeactivateHitBox()
        {
            hitBox.enabled = false;
        }

        private void ParentCurrentWeapon(Transform parent)
        {
            if (currentWeapon.transform.parent == parent)
            {
                return;
            }

            currentWeapon.transform.SetParent(parent);
            currentWeapon.transform.localPosition = Vector3.zero;
            currentWeapon.transform.localRotation = Quaternion.identity;
        }

        private void HandleWeaponScroll()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel"); // Legacy Input Manager axis
            if (scroll > 0f) CycleWeapon(+1);
            else if (scroll < 0f) CycleWeapon(-1);
        }

        public void CycleWeapon(int dir)
        {
            // Assuming exactly 3 entries in WeaponType enum
            int next = ((int)currentEquipment + dir + totalEquipment) % totalEquipment;
            currentEquipment = (EquipmentType)next;
        }

        public void StartAttackWindow(float duration)
        {
            StartCoroutine(AttackWindowCoroutine(duration));
        }

        private IEnumerator AttackWindowCoroutine(float duration)
        {
            sword.EnableAttack();
            yield return new WaitForSeconds(duration);
            sword.DisableAttack();
        }

        public void TakeDamage(float damage)
        {
            if (isBlocking)
            {
                damage = damage / 2;
            }
            playerHP -= damage;
            if (playerHP <= 0f)
            {   
                movementSM.ChangeState(die); // Change to die state
            }
            else
            {   
                if (isBlocking)
                {
                    anim.applyRootMotion = false; // Disable root motion for regular damage
                    anim.SetTrigger("Hit"); // Trigger block hit animation
                    SoundManager.Instance.PlaySound(SoundManager.Instance.block); // Play block hit sound
                }
                anim.applyRootMotion = false; // Disable root motion for regular damage
                anim.SetTrigger("Hit");
                SoundManager.Instance.PlaySound(SoundManager.Instance.playerHurt); // Play hit sound
            }
        }
        #endregion

        #region MonoBehaviour Callbacks

        private void Awake()
        {
            if (data == null)
            {
                Debug.LogError("CharacterData is not assigned in the inspector.");
            }
        }
        private void Start()
        {
            movementSM = new StateMachine();

            standing = new StandingState(this, movementSM);
            ducking = new DuckingState(this, movementSM);
            jumping = new JumpingState(this, movementSM);
            die = new DieState(this, movementSM);

            attackSM = new StateMachine();
            drawSword = new DrawSwordState(this, attackSM);
            swingSword = new SwingSwordState(this, attackSM);
            sheathSword = new SheathSwordState(this, attackSM);
            magic = new MagicState(this, attackSM);
            attackIdle = new AttackIdleState(this, attackSM); // New idle attack state

            movementSM.Initialize(standing);
            attackSM.Initialize(attackIdle); // Initialize attack state machine with DrawSwordState
        }

        private void Update()
        {   
            HandleWeaponScroll();

            movementSM.CurrentState.HandleInput();
            movementSM.CurrentState.LogicUpdate();

            attackSM.CurrentState.HandleInput();
            attackSM.CurrentState.LogicUpdate();

            if (currentWeapon != null)
            {
                sword = currentWeapon.GetComponent<Sword>();
            }
        }

        private void FixedUpdate()
        {
            movementSM.CurrentState.PhysicsUpdate();

            attackSM.CurrentState.PhysicsUpdate();
        }

        #endregion
    }
}
