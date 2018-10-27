using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Damagable : MonoBehaviour {

    public float Health = 100;
    public float BaseResistance = 20;
    public List<string> Tags = new List<string>() { "default" };

    public float ReceiveDamage( Damage dmg )
    {
        float receivedDMG = 0f;

        if ( Tags.Contains("untouchable") ) //object cannot take damage in any form if it has "untouchable" tag
            return Health;

        if ( dmg.TargetsTags.Any( Tags.Contains ) ) //if damage tags contain any of tags of the damagable object
        {
            receivedDMG = GetDmgValue(dmg);

            if (receivedDMG > 0)
            {
                Health -= receivedDMG;

                if (Health < 0)
                {
                    Kill();
                }
            }

        }

        OnAttacked(receivedDMG);
        return Health;
    }

    virtual protected void OnAttacked( float damageReceived ) { } //May be usefull in derived classes

    virtual protected void Kill() //probably will be overrided
    {
        Destroy(gameObject);
    }

    private float GetDmgValue( Damage dmg )
    {
        return Mathf.Clamp((dmg.BaseDamage - BaseResistance), 0, Health) + dmg.PiercingDamage;
    }
}
