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

    public class GunScript : PartModifierScript<GunData>,IFlightFixedUpdate,IFlightStart
    {
        ParticleSystem gun;
        ParticleSystem.Particle[] bullets;
        Collider gunCollider;

        public void FlightStart(in FlightFrameData frame)
        {
            gun = GetComponentInChildren<ParticleSystem>();
            gunCollider = PartScript.Colliders[0].Collider;
        }

        public void FlightFixedUpdate(in FlightFrameData frame)
        {
            bullets = new ParticleSystem.Particle[gun.particleCount];
            gun.GetParticles(bullets);
            for(int i = 0;i<bullets.Length; i++)
            { 
                RaycastHit hit;
                if(Physics.Raycast(bullets[i].position, bullets[i].velocity, out hit, bullets[i].velocity.magnitude / frame.DeltaTime))
                {
                    try//Bad practice. Never use exception handling for flow control.
                    {
                        if (hit.collider != gunCollider)
                        {
                            hit.collider.GetComponentInParent<PartScript>().TakeDamage(1, false);
                            hit.rigidbody.AddForceAtPosition(bullets[i].velocity * 0.01f, hit.point,ForceMode.Impulse);
                        }
                    }
                    catch (Exception) { }
                    bullets[i].remainingLifetime = 0;
                }
            }
            if (PartScript.Data.Activated)
                gun.Play();
        }
    }
}