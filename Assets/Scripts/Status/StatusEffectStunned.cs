using UnityEngine;

public class StatusEffectStunned : StatusEffect
{
    public StatusEffectStunned(int duration)
        : base(StatusEffectType.Stunned, duration, true) { }
}
