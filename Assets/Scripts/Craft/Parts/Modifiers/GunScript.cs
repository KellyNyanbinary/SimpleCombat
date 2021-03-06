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
    {
        ParticleSystem gun;
        ParticleSystem.Particle[] bullets;
        Collider gunCollider;
        bool firing;
        float reloadProgress = 0;
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
            if (PartScript.Data.Activated)
                gunEmission.rateOverTime = Data.rateOfFire;
            else gunEmission.rateOverTime = 0;

            Debug.Log(gunEmission.rateOverTime.constant + "," + gun.isPlaying);
            bullets = new ParticleSystem.Particle[gun.particleCount];
            gun.GetParticles(bullets);
            for (int i = 0; i < bullets.Length; i++)
            {
                if (bullets[i].remainingLifetime > Time.deltaTime)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(bullets[i].position, bullets[i].velocity, out hit, bullets[i].velocity.magnitude * frame.DeltaTime))
                    {
                        Debug.Log(bullets[i].velocity.magnitude * frame.DeltaTime);
                        if (hit.collider != gunCollider)
                        {
                            if (hit.collider.GetComponentInParent<PartScript>())
                            {
                                hit.collider.GetComponentInParent<PartScript>().TakeDamage(Data.damage, false);
                                hit.rigidbody.AddForceAtPosition(bullets[i].velocity * Data.mass, hit.point, ForceMode.Impulse);
                                Debug.Log(hit.collider.GetComponentInParent<PartScript>().name);
                            }
                            bullets[i].remainingLifetime = Time.deltaTime;
                        }
                    }
                    else bullets[i].position += bullets[i].velocity * frame.DeltaTime;
                }
            }
            gun.SetParticles(bullets);
        }

        public void FlightUpdate(in FlightFrameData frame)
        {
            bullets = new ParticleSystem.Particle[gun.particleCount];
            gun.GetParticles(bullets);
            for (int i = 0; i < bullets.Length; i++)
            {
                bullets[i].position -= bullets[i].velocity * frame.DeltaTime;
            }
            gun.SetParticles(bullets);
        }

        System.Collections.IEnumerator FireGun()//unused
        {
            firing = true;
            while (PartScript.Data.Activated)
            {
                gun.Play();
                yield return new WaitForSeconds(1 / Data.rateOfFire / Time.timeScale);
            }
            firing = false;
            yield return null;
        }
    }
}