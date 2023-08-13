using UnityEngine;
using UnityEngine.UI;

namespace BattleSimulator
{
    public class PreGameUI : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private SpawnScriptableObject config;

        [Header("Start")]
        [SerializeField] private Button startButton;

        [Header("Enemy Team")]
        [SerializeField] private TeamButton teamButtonPrefab;
        [SerializeField] private Transform teamButtonContainer;

        private void Awake()
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }

        private void Start()
        {
            foreach (var teamConfig in config.Teams)
            {
                InitializeEnemyTeamUI(teamConfig);
            }
        }

        public void InitializeEnemyTeamUI(SpawnSetupTeam teamConfig)
        {
            var teamButton = Instantiate(teamButtonPrefab, teamButtonContainer);
            teamButton.Initialize(teamConfig.Id, teamConfig.Name);
        }

        private void OnStartButtonClicked()
        {
            DomainEvents.Raise(new GameStartedEvent());
            gameObject.SetActive(false);
        }
    }
}
