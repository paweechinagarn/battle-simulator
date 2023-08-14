using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BattleSimulator
{
    public class TeamButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI text;

        private int playerId;
        private int teamId;

        public void Initialize(int playerId, int teamId, string name)
        {
            this.playerId = playerId;
            this.teamId = teamId;
            text.text = name;
        }

        private void Awake()
        {
            button.onClick.AddListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            DomainEvents.Raise(new TeamSelectedEvent
            {
                PlayerId = playerId,
                TeamId = teamId 
            });
        }
    }
}
