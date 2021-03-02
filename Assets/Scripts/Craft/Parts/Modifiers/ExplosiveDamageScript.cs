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
        public void FlightUpdate(in FlightFrameData frame)
        {
            position = PartScript.Transform.position;
        }

        public override void OnPartDestroyed()
        {
            Debug.Log("destroy!");
            Collider[] colliders = Physics.OverlapSphere(position, Data.explosionRadius);
            Debug.Log("Got Hit!");
            foreach (Collider collider in colliders)
            {
                Debug.Log("Iterating");
                if (collider.GetComponent<ModApi.Craft.Parts.PartColliderScript>().IsPrimary)
                {
                    try
                    {
                        collider.GetComponentInParent<PartScript>().TakeDamage(Data.explosionDamage, false);
                        collider.attachedRigidbody.AddForceAtPosition((collider.transform.position - position).normalized * Data.explosivePower, collider.transform.position, ForceMode.Impulse);
                        Debug.Log(collider.gameObject.name);
                    }
                    catch (Exception) { Debug.Log("except"); }
                }
            }
        }
    }
}