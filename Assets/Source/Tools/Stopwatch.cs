using System;

namespace DIGA7009A.Tools
{
  public class Stopwatch
  {
    public enum MeasurementUnit
    {
      Seconds,
      Milliseconds
    }

    private MeasurementUnit measurementUnit = MeasurementUnit.Seconds;

    //----------------------------------------------------------------------------------------------------------------------

    public Stopwatch( MeasurementUnit measurementUnit )
    {
      this.measurementUnit = measurementUnit;
    }

    //----------------------------------------------------------------------------------------------------------------------

    public Stopwatch( float time )
    {
      TimerDuration = time;
    }

    //----------------------------------------------------------------------------------------------------------------------

    public float TimerDuration
    {
      get
      {
        return duration;
      }
      set
      {
        duration = Math.Max( 0, value );
      }
    }

    //----------------------------------------------------------------------------------------------------------------------

    public float ElapsedTime
    {
      get
      {
        if( measurementUnit == MeasurementUnit.Seconds )
        {
          return elapsedTime;
        }
        return elapsedTime / 1000.0f;
      }
    }

    //----------------------------------------------------------------------------------------------------------------------

    public bool Running
    {
      get; private set;
    }

    //----------------------------------------------------------------------------------------------------------------------

    private float duration;

    private float elapsedTime;

    //----------------------------------------------------------------------------------------------------------------------

    public void DoLogic( float deltaTime )
    {
      if( Running )
      {
        elapsedTime += deltaTime;
      }
    }

    //----------------------------------------------------------------------------------------------------------------------

    public bool HasElapsed()
    {
      if( elapsedTime > duration )
      {
        elapsedTime = 0f;
        return true;
      }

      return false;
    }

    //----------------------------------------------------------------------------------------------------------------------

    public void Start()
    {
      Running = true;
    }

    //----------------------------------------------------------------------------------------------------------------------

    public void Stop()
    {
      Running = false;
      elapsedTime = 0.0f;
    }

    //----------------------------------------------------------------------------------------------------------------------

    public void Pause()
    {
      Running = false;
    }

    //----------------------------------------------------------------------------------------------------------------------
  }
}


