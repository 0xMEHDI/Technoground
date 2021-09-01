using SGoap.Services;
using UnityEngine;

namespace SGoap
{
    /// <summary>
    /// A basic extension of a typical agent, providing generic features such as Inventory, animator controller and stagger functionality.
    /// </summary>
    public class BasicAgent : Agent, ITarget, IAttacker
    {
        private EffectController _effectsController;
        public AgentBasicData Data;
        
        private void Awake()
        {
            Initialize();
            TargetManager.Add(this);
        }

        private void OnDestroy()
        {
            TargetManager.Remove(this);
        }

        public void Initialize()
        {
            var animator = GetComponentInChildren<Animator>();
            _effectsController = GetComponentInChildren<EffectController>();

            Data = new AgentBasicData
            {
                Animator = animator,
                EffectsController = _effectsController,
                Agent = this,
                Inventory = new Inventory(),
                Cooldown = new CoolDown(),
            };

            var targetDependencies = GetComponentsInChildren<IDataBind<AgentBasicData>>();
            foreach (var dependency in targetDependencies)
                dependency.Bind(Data);
        }

        public override void LateUpdate()
        {
            if (!Data.Cooldown.Active)
                base.LateUpdate();
        }
    }

    public interface ITarget
    {
        Transform transform { get; }
    }
}

public enum ETeam
{
    Blue,
    Red,
    None
}

public interface ITeam
{
    ETeam Team { get; }
}