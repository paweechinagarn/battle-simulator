using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BattleSimulator
{
    public class TeamButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI text;

        private int id;

        public void Initialize(int id, string name)
        {
            this.id = id;
            text.text = name;
        }

        private void Awake()
        {
            button.onClick.AddListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            DomainEvents.Raise(new TeamSelectedEvent { Id = id });
        }
    }
}
