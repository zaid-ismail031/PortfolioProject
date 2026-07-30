using StarterAssets;
using UnityEngine;

namespace Source.DecisionMake
{
    public class NPCDecisionTree : MonoBehaviour
    {
        
        [SerializeField] private ThirdPersonController playerCharacter;
        [SerializeField] private NPCController npcController;
    [SerializeField] private HealthPickup healthPickup;
        //[SerializeField] private CharacterHealth characterHealth;
        
        private DecisionTreeNode _rootNode;
        
        //update variables, so we check after each tick rather than each frame
        private float _counter;
        private const float TickInterval = 0.1f;
        
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            InitDecisionTree();
            
        }

        // Update is called once per frame
        private void Update()
        {
            
            _counter += Time.deltaTime;
            
            //Evaluates the decision tree each frame
            //Make Decision recursively evaluates decisions along tree

            if (_counter >= TickInterval)
            {
                //Debug.Log("Tick");
                _counter = 0;

                if (_rootNode != null)
                {
                    //_root node is not null
                    _rootNode.MakeDecision();
                }
                
            }
            
        }

        private void InitDecisionTree()
        {
            //Creating actions
            var chaseAction = new ChaseAction();
            chaseAction.Init(npcController);
            
            var punchAction = new PunchAction();
            punchAction.Init(npcController);
            
            var fleeAction = new FleeAction();
            fleeAction.Init(npcController);
            
            var patrolAction = new PatrolAction();
            patrolAction.Init(npcController);
            
            //goTo Addition
            var goToAction = new GoToAction();
            goToAction.Init(npcController);
            
            //Creating Decisions and tree
            var healthDecision = new HealthDecision(0.4f,npcController);
            var punchDecision = new PunchDistanceDecision(1.5f,playerCharacter,npcController);
            var sightDecision = new SightDistanceDecision(5f,playerCharacter,npcController);

            //go To Addition
            var pickupDecision = new PickUpDecision(goToAction,npcController, healthPickup );

            //setting up the actual tree structure
            //Setting up true and false nodes

            //setting up the root node 
            _rootNode = healthDecision;
            
            //if sufficient health to patrol, and sight, chase and try punch
            healthDecision.SetNodes(sightDecision,pickupDecision);
            sightDecision.SetNodes(punchDecision,patrolAction);
            punchDecision.SetNodes(punchAction, chaseAction);
            //else check pickup, or keep fleeing
            pickupDecision.SetNodes(goToAction, fleeAction);
            

        }


        
    }
}
