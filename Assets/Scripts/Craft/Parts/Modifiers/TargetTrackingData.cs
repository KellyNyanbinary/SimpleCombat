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
    [DesignerPartModifier("TargetTracking")]
    [PartModifierTypeId("SimpleCombat.TargetTracking")]
    public class TargetTrackingData : PartModifierData<TargetTrackingScript>
    {
    }
}