using System;
using System.Threading.Tasks;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Assertions.Must;
using static UnityEngine.GraphicsBuffer;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Attack", story: "[Self] attacks", category: "Action", id: "890316c3ebae7973555d747e360a96d3")]
public partial class AttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    private float lastAttackTime;
    private float attackCoolDown = 1.0f; // Cooldown time between attacks

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Self.Value == null)
        {
            return Status.Failure; // No target
        }
        if (Time.time >= lastAttackTime + attackCoolDown)
        {
            Self.Value.GetComponent<Monster>().Attack();

            lastAttackTime = Time.time; // Update last attack time
            return Status.Running; // Still in cooldown period
        }
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

