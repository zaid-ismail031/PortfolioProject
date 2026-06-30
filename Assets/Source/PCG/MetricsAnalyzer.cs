using UnityEngine;
using System;
using System.Collections.Generic;
using Random = System.Random;

namespace Source.PCG
{
    public class MetricsAnalyzer : MonoBehaviour
    {
        
        [SerializeField] private DungeonGenerator dungeons;
        
        [SerializeField] private GameObject enemySpawner;
        [SerializeField] private GameObject resourceSpawner;
        
        
        //Metrics
        [SerializeField] private MetricsData metricsData;

        public static event Action<MetricsData> DisplayData;
        
        
        
        //[SerializeField] private MetricsDisplay metricsDisplay;
        
        public void OnEnable()
        {
            
        }


        public void OnDisable()
        {
            
        }
        
        
        

        public void ComputeMetrics()
        {
            if (!dungeons) { return; }
            
            var rooms = dungeons.GetRooms();

            var roomAdjacency = dungeons.GetAdjacency();
            
            
            var startRoomIndex = dungeons.GetStartRoomIndex();
            var goalRoomIndex = dungeons.GetGoalRoomIndex();
            
            
            metricsData.TotalRooms = rooms.Count;
            metricsData.TotalCorridors = GetCorridors();
            
            metricsData.IsReachable = IsReachable(startRoomIndex, goalRoomIndex, roomAdjacency);
        
         
            metricsData.AveragePathLength = ComputeAveragePathLength(rooms.Count, roomAdjacency);
            
            metricsData.DeadEndFrequency = ComputeDeadEndFrequency(roomAdjacency);
        
            
            //Will calculate enemy density
            metricsData.TotalEnemies = GetObjectCount("Enemy");
            metricsData.EnemyDensity = (float)metricsData.TotalEnemies / rooms.Count;
        
            //Will calculate 
            metricsData.TotalResources = GetObjectCount("Resource");
            metricsData.ResourceDistribution = (float)metricsData.TotalResources / rooms.Count;
            
            //Display UI
            DisplayData?.Invoke(metricsData);

        }


        public void LogAnalytics()
        {
            metricsData.PrintAnalytics();
        }



        private int GetCorridors()
        {
            DungeonGenerator.CellType[,] grid = dungeons.GetGrid();
            int count = 0;

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j] == DungeonGenerator.CellType.Corridor)
                    {
                        count++;
                    }
                }
            }

            return count;

        }


        private bool IsReachable(int startIndex, int goalIndex, List<List<int>> adjacency)
        {
            //Search for goal room through adjacency structure
            if (startIndex == goalIndex)
            {
                return true;
            }

            List<int> frontier = new List<int>();
            List<int> visited = new List<int>();
            
            frontier.Add(startIndex);
            visited.Add(startIndex);
            
           
            while (frontier.Count > 0)
            {
           
                int current = frontier[0];
                frontier.RemoveAt(0);

                //for connected room, search for our goal
                foreach (int adjRoom in adjacency[current])
                {
                    if (adjRoom == goalIndex)
                    {
                        return true;
                    }

                    if (!visited.Contains(adjRoom))
                    {
                        frontier.Add(adjRoom);
                        visited.Add(adjRoom);
                    }
                    
                }
                
            }
            
            return false;
            
        }



        private float ComputeAveragePathLength(int roomCount, List<List<int>> adjacency)
        {
            
            var sampleCount = Mathf.Min(roomCount, 10);
            float totalDistance = 0;
            var pairCount = 0;
    
            
            var rand = new Random(dungeons.GetSeed());
    
            for (var i = 0; i < sampleCount; i++)
            {
                
                var randomStartingRoom = rand.Next(roomCount);
        
                // BFS from start to all other reachable rooms
                var distances = new Dictionary<int, int>();
                var frontier = new List<int> { randomStartingRoom };

                distances[randomStartingRoom] = 0;
        
                while (frontier.Count > 0)
                {
                    var current = frontier[0];
                    frontier.RemoveAt(0);
            
                    foreach (var adjRoom in adjacency[current])
                    {
                        if (adjRoom == current)
                        {
                            continue;
                        }
                        
                        
                        if (!distances.ContainsKey(adjRoom))
                        {
                            distances[adjRoom] = distances[current] + 1;
                            frontier.Add(adjRoom);
                        }
                    }
                }
                
                foreach (var path in distances)
                {
                    totalDistance += path.Value;
                    pairCount++;
                }
            }

            if (pairCount > 0)
            {
                totalDistance = totalDistance / pairCount;
            }
            else
            {
                totalDistance = 0;
            }
    
            return totalDistance;
            
         
        }


        private float ComputeDeadEndFrequency(List<List<int>> adjacency)
        {
            int count = 0;

            for (var i = 0; i < adjacency.Count; i++)
            {
                if (adjacency[i].Count <= 1)
                {
                    count++;
                }
            
            }

            if (adjacency.Count > 0)
            {
                return (float)count/adjacency.Count;
            }
          
            return 0;
            
            
        }


        private int GetObjectCount(string tag)
        {
            return GameObject.FindGameObjectsWithTag(tag).Length;
            return 0;
        }
        
        
        private void Update()
    {
      if( Input.GetKeyDown(KeyCode.G))
      {
        ComputeMetrics();
      }
    }
        
        

    }
        
        
        
        
        
        
        
        
        
        
        
    
}
