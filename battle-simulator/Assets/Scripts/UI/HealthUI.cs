using TMPro;
using UnityEngine;

namespace BattleSimulator
{
    public class HealthUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;

        public int Health
        {
            set
            {
                text.text = value.ToString();
            }
        }
    }
}
