namespace Assets.Scripts.Craft.Parts.Modifiers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using ModApi.Craft.Parts;
    using ModApi.Craft.Parts.Attributes;
    using UnityEngine;

    [Serializable]
    [DesignerPartModifier("Gun")]
    [PartModifierTypeId("SimpleCombat.Gun")]
    public class GunData : PartModifierData<GunScript>
    {
        [DesignerPropertySlider(Label = "Damage", MinValue = 1, MaxValue = 20, NumberOfSteps = 20)]
        public float damage = 1;

        [DesignerPropertySlider(Label = "Bullet Mass", MinValue = 0.01f, MaxValue = 0.2f, NumberOfSteps = 20)]
        public float bulletMass = 0.01f;

        [DesignerPropertySlider(Label = "Muzzle Velocity", MinValue = 200, MaxValue = 2000, NumberOfSteps = 10)]
        public float muzzleVelocity = 1000;

        [DesignerPropertySlider(Label = "Rate of Fire", MinValue = 20, MaxValue = 100, NumberOfSteps = 17)]
        public float rateOfFire = 50;

        [DesignerPropertySlider(Label = "Bullet Dispersion", MinValue = 0, MaxValue = 1)]
        public float bulletDispersion = 0.1f;

        [DesignerPropertySlider(Label = "Recoil Multiplier",MinValue =0f,MaxValue =1.2f, NumberOfSteps = 13)]
        public float recoil = 0f;
    }
}