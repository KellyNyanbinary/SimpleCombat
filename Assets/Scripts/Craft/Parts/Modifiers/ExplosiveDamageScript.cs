namespace Assets.Scripts.Craft.Parts.Modifiers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using ModApi.Craft.Parts;
    using ModApi.GameLoop;
    using ModApi.GameLoop.Interfaces;
    using UnityEngine;

    public class ExplosiveDamageScript : PartModifierScript<ExplosiveDamageData>,IFlightUpdate
    {
        Vector3 position;
        ParticleSystem bullets;
        public void FlightUpdate(in FlightFrameData frame)
        {
            position = PartScript.Transform.position;
        }

        public override void OnPartDestroyed()
        {
            Collider[] colliders = Physics.OverlapSphere(position, Data.explosionRadius);//gets colliders in explosion range
            foreach (Collider collider in colliders)
            {
                try//Bad practice. Don't use exception handling for flow control. Will change to null testing in the future
                {
                    if (collider.GetComponent<ModApi.Craft.Parts.PartColliderScript>().IsPrimary)//Only adding force to primary colliders to avoid adding multiple forces to the same part. Could be replaced with weighing collider size in the future
                    {
                        collider.GetComponentInParent<PartScript>().TakeDamage(Data.explosionDamage, false);//adds damage
                        collider.attachedRigidbody.AddExplosionForce(Data.explosivePower, position, Data.explosionRadius,0 , ForceMode.Impulse);//adds impulse
                    }//still need to add attenuation
                }
                catch (Exception) { }
            }
        }
    }
}