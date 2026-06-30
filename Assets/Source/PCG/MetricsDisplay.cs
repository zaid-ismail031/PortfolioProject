using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Source.PCG
{
    public class MetricsDisplay : MonoBehaviour
    {
        
        public void OnEnable()
        {
            MetricsAnalyzer.DisplayData += UpdateDisplay;
        }


        public void OnDisable()
        {
            MetricsAnalyzer.DisplayData -= UpdateDisplay;
        }
        
        
        [SerializeField] private TMP_Text isReachableText;
        [SerializeField] private TMP_Text averagePathLengthText;
        [SerializeField] private TMP_Text enemyDensityText;
        [SerializeField] private TMP_Text resourceDistributionText;
        [SerializeField] private TMP_Text deadEndFrequencyText;


        private void UpdateDisplay(MetricsData data)
        {
            if (!data) return;

            var reachable = data.IsReachable ? "Yes":"No";

            isReachableText.text         = $"{reachable}";
            averagePathLengthText.text   = $"{data.AveragePathLength:F2}";
            enemyDensityText.text        = $"{data.EnemyDensity:F2}";
            resourceDistributionText.text = $"{data.ResourceDistribution:F2}";
            deadEndFrequencyText.text    = $"{data.DeadEndFrequency:F2}";
        }
    }
}
