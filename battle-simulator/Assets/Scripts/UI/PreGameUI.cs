using UnityEngine;
using UnityEngine.UI;

namespace BattleSimulator
{
    public class PreGameUI : MonoBehaviour, IDomainEventHandler<PreGameEvent>
    {
        [Header("Start")]
        [SerializeField] private Button startButton;

        [Header("Enemy Team")]
        [SerializeField] private PlayerSpawnSetup config;
        [SerializeField] private int playerId;
        [SerializeField] private TeamButton teamButtonPrefab;
        [SerializeField] private Transform teamButtonContainer;

        private void Awake()
        {
            DomainEvents.RegisterDomainEventHandler(this);
            startButton.onClick.AddListener(OnStartButtonClicked);
        }

        private void Start()
        {
            foreach (var teamConfig in config.Teams)
            {
                InitializeEnemyTeamUI(teamConfig);
            }
        }

        private void OnDestroy()
        {
            DomainEvents.UnregisterDomainEventHandler(this);
        }

        public void InitializeEnemyTeamUI(SpawnSetupTeam teamConfig)
        {
            var teamButton = Instantiate(teamButtonPrefab, teamButtonContainer);
            teamButton.Initialize(playerId, teamConfig.Id, teamConfig.Name);
        }

        private void OnStartButtonClicked()
        {
            DomainEvents.Raise(new StartGameRequestEvent());
            gameObject.SetActive(false);
        }

        public void Handle(PreGameEvent evt)
        {
            gameObject.SetActive(true);
        }
    }
}
