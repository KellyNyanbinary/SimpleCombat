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

    public class RotaryGunScript : PartModifierScript<RotaryGunData>, IDesignerUpdate,IDesignerStart,IFlightUpdate,IFlightStart
    {
        int numOfBarrels = 0;
        float rotateSpeed = 0;
        Transform barrels;
        public void FlightStart(in FlightFrameData frame)
        {
            numOfBarrels = 0;
            UpdateBarrels(Data.numberOfBarrels);
            barrels = transform.Find("Barrels");
        }
        
        public void DesignerStart(in DesignerFrameData frame)
        {
            numOfBarrels = 0;
            UpdateBarrels(Data.numberOfBarrels);
        }

        public void DesignerUpdate(in DesignerFrameData frame)
        {
            if (frame.Designer.SelectedPart == PartScript)
                UpdateBarrels(Data.numberOfBarrels);
        }

        public void FlightUpdate(in FlightFrameData frame)
        {
            if (PartScript.Data.Activated)
                rotateSpeed = GetComponent<GunScript>().Data.rateOfFire / Data.numberOfBarrels;
            else rotateSpeed *= Mathf.Pow(0.1f, (float)frame.DeltaTimeWorld);
            barrels.Rotate(Vector3.up * rotateSpeed * 360 * (float)frame.DeltaTimeWorld);
        }

        private void UpdateBarrels(int numberOfBarrels)
        {
            if (numberOfBarrels != numOfBarrels)
            {
                Transform[] barrels = PartScript.Transform.Find("Barrels").GetComponentsInChildren<Transform>();
                for (int i = 0; i < barrels.Length; i++)
                {
                    if (barrels[i].gameObject.name != "0"&& barrels[i].gameObject.name != "Barrels")
                        Destroy(barrels[i].gameObject);
                }
                Transform barrel;
                for (int i = 1; i <= numberOfBarrels; i++)
                {
                    barrel = Instantiate(PartScript.Transform.Find("Barrels").Find("0"), PartScript.Transform.Find("Barrels"));
                    barrel.gameObject.name = i.ToString();
                    barrel.localPosition = Data.barrelSeperation * (Vector3.right * Mathf.Cos(2 * Mathf.PI * ((float)i / (float)numberOfBarrels)) + Vector3.forward * Mathf.Sin(2 * Mathf.PI * ((float)i / (float)numberOfBarrels)));
                }
                Debug.Log(numberOfBarrels);
                numOfBarrels = numberOfBarrels;
            }
        }

    }
}