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

    public class ExplosiveDamageScript : PartModifierScript<ExplosiveDamageData>,IFlightStart
    {
        Vector3 position;
        ParticleSystem bullets;
        bool armed = false;
        public void FlightStart(in FlightFrameData frame)
        {
            armed = true;
        }

        public override void OnPartDestroyed()
        {
            if (!armed)
                return;
            Collider[] colliders = Physics.OverlapSphere(transform.position, Data.explosionRadius);//gets colliders in explosion range
            foreach (Collider collider in colliders)
            {
                if (collider.GetComponent<ModApi.Craft.Parts.PartColliderScript>())
                {
                    if (collider.GetComponent<ModApi.Craft.Parts.PartColliderScript>().IsPrimary)//Only adding force to primary colliders to avoid adding multiple forces to the same part. Could be replaced with weighing collider size in the future
                    {
                        RaycastHit hit;
                        Physics.Raycast(transform.position, collider.bounds.center - transform.position, out hit);
                        if (hit.collider == collider)//if not blocked
                        {
                            collider.GetComponentInParent<PartScript>().TakeDamage(Data.explosionDamage, false);//adds damage
                            collider.attachedRigidbody.AddExplosionForce(Data.explosivePower, position, Data.explosionRadius, 0, ForceMode.Impulse);//adds impulse
                        }
                    }//still need to add attenuation
                }
            }
        }
    }
}