using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLogic : MonoBehaviour
{
    [SerializeField] protected GameObject projectileSample;
    [SerializeField] protected float coolDown;
    [SerializeField] protected ItemsBase ammoType;
    protected bool readyToFire = true;

    public bool ReadyToFire
    {
        get { return readyToFire;  }
    }

    protected IEnumerator GoCoolDown()
    {
        readyToFire = false;
        yield return new WaitForSeconds(coolDown);
        readyToFire = true;
    }

    public ItemsBase AmmoType
    {
        get { return ammoType; }
    }

    virtual public void UseWeapon() {}

    virtual public void StopWeapon() {}
}
