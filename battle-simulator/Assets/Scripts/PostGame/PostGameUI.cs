using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BattleSimulator
{
    public class PostGameUI : MonoBehaviour, IDomainEventHandler<GameEndedEvent>
    {
        [Header("Result")]
        [SerializeField] private TextMeshProUGUI resultText;
        [SerializeField] private string winText;
        [SerializeField] private string loseText;

        [Header("Game State")]
        [SerializeField] private Button mainMenuButton;

        private void Awake()
        {
            DomainEvents.RegisterDomainEventHandler(this);
            mainMenuButton.onClick.AddListener(GoToMainMenu);
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            DomainEvents.UnregisterDomainEventHandler(this);
        }

        private void GoToMainMenu()
        {
            DomainEvents.Raise(new PreGameRequestEvent());
            gameObject.SetActive(false);
        }

        public void Handle(GameEndedEvent evt)
        {
            gameObject.SetActive(true);
            resultText.text = evt.IsWon ? winText : loseText;
        }
    }
}
