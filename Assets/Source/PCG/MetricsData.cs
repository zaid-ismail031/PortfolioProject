using UnityEngine;

namespace Source.PCG
{
    [CreateAssetMenu(fileName = "MetricsData", menuName = "ScriptableObjects/MetricsData")]
    public class MetricsData : ScriptableObject
    { //Metrics
        
        [SerializeField] private bool isReachable;
        [SerializeField] private float averagePathLength;
        [SerializeField] private int totalRooms;
        [SerializeField] private int totalCorridors;
        [SerializeField] private int totalEnemies;
        [SerializeField] private int totalResources;
        [SerializeField] private float enemyDensity;
        [SerializeField] private float resourceDistribution;
        [SerializeField] private float deadEndFrequency;
     
        
        public bool IsReachable
        {
            get => isReachable;
            set => isReachable = value;
        }

        public float AveragePathLength
        {
            get => averagePathLength;
            set => averagePathLength = value;
        }

        public int TotalRooms
        {
            get => totalRooms;
            set => totalRooms = value;
        }

        public int TotalCorridors
        {
            get => totalCorridors;
            set => totalCorridors = value;
        }

        public int TotalEnemies
        {
            get => totalEnemies;
            set => totalEnemies = value;
        }

        public int TotalResources
        {
            get => totalResources;
            set => totalResources = value;
        }

        public float EnemyDensity
        {
            get => enemyDensity;
            set => enemyDensity = value;
        }

        public float ResourceDistribution
        {
            get => resourceDistribution;
            set => resourceDistribution = value;
        }

        public float DeadEndFrequency
        {
            get => deadEndFrequency;
            set => deadEndFrequency = value;
        }


        public string PrintAnalytics()
        {

            string output = $"Is Reachable: {IsReachable}\n" +
                     $"Average Path Length: {AveragePathLength}\n" +
                     $"Total Rooms: {TotalRooms}\n" +
                     $"Total Corridors: {TotalCorridors}\n" +
                     $"Enemy Density: {EnemyDensity}\n" +
                     $"Resource Distribution: {ResourceDistribution}\n" +
                     $"Dead End Frequency: {DeadEndFrequency}";


            Debug.Log(output);
            return output;
            
        }
        
        
        
        
        
    }
}
