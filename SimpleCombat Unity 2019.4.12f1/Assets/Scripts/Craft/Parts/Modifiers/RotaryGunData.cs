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
    [DesignerPartModifier("RotaryGun")]
    [PartModifierTypeId("SimpleCombat.RotaryGun")]
    public class RotaryGunData : PartModifierData<RotaryGunScript>
    {
        [DesignerPropertySlider(Label = "Number of barrels", MinValue = 3, MaxValue = 12, NumberOfSteps = 10)]
        public int numberOfBarrels = 3;
        [DesignerPropertySlider(Label = "Barrel seperation", MinValue = 0.1f, MaxValue = 0.5f)]
        public float barrelSeperation = 0.1f;
    }
}