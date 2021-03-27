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
    [DesignerPartModifier("ExplosiveDamage")]
    [PartModifierTypeId("SimpleCombat.ExplosiveDamage")]
    public class ExplosiveDamageData : PartModifierData<ExplosiveDamageScript>
    {
        [DesignerPropertySlider(Label = "Explosive Power", MinValue = 0, MaxValue = 100)]
        public float explosivePower;
        [DesignerPropertySlider(Label = "Explosion Radius", MinValue = 0, MaxValue = 100)]
        public float explosionRadius;
        [DesignerPropertySlider(Label = "Explosion Damage", MinValue = 0, MaxValue = 100)]
        public float explosionDamage;
    }
}