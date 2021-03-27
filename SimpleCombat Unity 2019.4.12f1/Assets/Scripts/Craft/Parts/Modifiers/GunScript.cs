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

    public class GunScript : PartModifierScript<GunData>, IFlightFixedUpdate, IFlightStart, IFlightUpdate
    {//Use of the particle buffer could still be optimized
        ParticleSystem gun;
        ParticleSystem.Particle[] bullets;
        Collider gunCollider;
        ParticleSystem.EmissionModule gunEmission;

        public void FlightStart(in FlightFrameData frame)
        {
            gun = GetComponentInChildren<ParticleSystem>();
            gunCollider = PartScript.Colliders[0].Collider;
            ParticleSystem.MainModule particleMain = gun.main;
            particleMain.startSpeed = Data.muzzleVelocity;
            ParticleSystem.ShapeModule particleShape = gun.shape;
            particleShape.angle = Data.bulletDispersion;
            gunEmission = gun.emission;
        }

        public void FlightFixedUpdate(in FlightFrameData frame)
        {
            if (PartScript.Data.Activated)//control firing and rate of fire by changing the rate the particle system creates particles
                gunEmission.rateOverTime = Data.rateOfFire;
            else gunEmission.rateOverTime = 0;

            PartScript.BodyScript.RigidBody.AddForceAtPosition(-PartScript.Transform.forward * Data.rateOfFire * Data.bulletMass * Data.muzzleVelocity * Data.recoil, PartScript.Transform.position);//apply recoil

            bullets = new ParticleSystem.Particle[gun.particleCount];//setup the buffer for handling particle data
            gun.GetParticles(bullets);
            for (int i = 0; i < bullets.Length; i++)//test for hits and update position for each bullet
            {
                if (bullets[i].remainingLifetime > Time.deltaTime)//if bullet has not hit anything in previous fixed update
                {//do a ray cast to check for collision
                    RaycastHit hit;
                    if (Physics.Raycast(bullets[i].position, bullets[i].velocity, out hit, bullets[i].velocity.magnitude * frame.DeltaTime))
                    {
                        if (hit.collider != gunCollider)//ignores collider on the gun in case the bullet somehow goes behind the gun
                        {
                            if (hit.collider.GetComponentInParent<PartScript>())//if hits a part
                            {
                                hit.collider.GetComponentInParent<PartScript>().TakeDamage(Data.damage, false);
                                hit.rigidbody.AddForceAtPosition(bullets[i].velocity * Data.bulletMass, hit.point, ForceMode.Impulse);
                            }
                            bullets[i].remainingLifetime = Time.deltaTime;//tell the particle system to destroy this particle on next update
                        }
                    }
                    else bullets[i].position += bullets[i].velocity * frame.DeltaTime;//moves the bullet forward
                }
            }
            gun.SetParticles(bullets);//apply changes to the particle system
        }

        public void FlightUpdate(in FlightFrameData frame)
        {//move each particle back to previous position because it's updated in this script on fixed updates.
            bullets = new ParticleSystem.Particle[gun.particleCount];
            gun.GetParticles(bullets);
            for (int i = 0; i < bullets.Length; i++)
            {
                bullets[i].position -= bullets[i].velocity * frame.DeltaTime;
            }
            gun.SetParticles(bullets);
        }
    }
}