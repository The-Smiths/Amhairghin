using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOverTime : DamagaDealer {
    public float DamageDelay = 1f;
    private List<Damagable> damagable = new List<Damagable>();

	// Use this for initialization
	void Start () {
        StartCoroutine("DealDamage");
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        Damagable damagableObject;
        if( damagableObject = other.GetComponent<Damagable>() )
        {
            damagable.Add( damagableObject );
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        damagable.RemoveAll( x => x == other.GetComponent<Damagable>() );
    }

    protected IEnumerator DealDamage()
    {
        while (true)
        {
            if (DamageObject)
                damagable.ForEach(x => x.ReceiveDamage(DamageObject));

            yield return new WaitForSeconds(DamageDelay);
        }
    }
}
