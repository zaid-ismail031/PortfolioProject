using DIGA7009A.Tools;
using UnityEngine;

namespace Source.States
{
    public class PatrolTimerCondition : Condition
    {
        private readonly Stopwatch stopwatch;

        public PatrolTimerCondition( float duration )
        {
            stopwatch = new Stopwatch( duration );
        }

        public override bool Test()
        {
            stopwatch.DoLogic( Time.deltaTime );

            if( stopwatch.HasElapsed() )
            {
                stopwatch.Stop();
                return true;
            }

            return false;
        }

        public void Reset()
        {
            stopwatch.Stop();
            stopwatch.Start();
        }
    }
}
