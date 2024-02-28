using System;
using System.Collections;
using UnityEngine;
using TMPro;

namespace AFSInterview.Units
{
    public class UnitUI : MonoBehaviour
    {
        // GO References
        public MeshRenderer mesh;
        public GameObject unitTurn;
        public Transform unitRange;
        public TMP_Text UnitName;
        public TMP_Text UnitDynamicText;
        public TMP_Text UnitHealth;
        public TMP_Text UnitArmor;

        private string name;
        private int maxHealth;
        private int maxArmor;
        
        // Set UI information and make sure transforms are faced to the camera
        public void SetData(string startName, int startHealth, int startArmor, float startRange, float uiEulerAnglesY)
        {
            name = startName;
            maxHealth = startHealth;
            maxArmor = startArmor;

            unitRange.transform.localScale = new Vector3(startRange * 2, unitRange.transform.localScale.y, startRange * 2);
            unitTurn.transform.localEulerAngles = new Vector3(0.0f, uiEulerAnglesY, 0.0f);
            UnitDynamicText.transform.localEulerAngles = new Vector3(30.0f, uiEulerAnglesY, 0.0f);

            UnitName.text = name;
            UpdateUI(maxHealth, maxArmor);
        }

        // Update UI
        public void UpdateUI(int newHealth, int newArmor)
        {
            UnitHealth.text = "Health: " + newHealth;
            UnitArmor.text = "Armor: " + newArmor;
        }

        // Display dynamic texts
        public void SetDynamicText(string newText, float duration = -1)
        {
            UnitDynamicText.text = newText;
            StartCoroutine(DisableDynamicText(duration));
        }

        // Disable current dynamic text
        private IEnumerator DisableDynamicText(float duration)
        {
            yield return new WaitForSeconds(Math.Abs(duration - (-1)) < 1.0f ? 4.5f : duration);
            UnitDynamicText.text = "";
        }
    }
}
