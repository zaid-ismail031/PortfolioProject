using System;
using System.Collections.Generic;
namespace DIGA7009A.Tools
{
  // Initiate a callback method when internalValue changes
  public class OnValueChangeCallback<T>
  {
    public T Value
    {
      get
      {
        return internalValue;
      }
      set
      {
        if( !equalityComparer.Equals( internalValue, value ) )
        {
          internalValue = value;
          onChangedCallback( value );
        }
      }
    }

    private T internalValue;
    private readonly Action<T> onChangedCallback = null;
    private readonly IEqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;

    public OnValueChangeCallback( T defaultValue, Action<T> onChangedCallback )
    {
      internalValue = defaultValue;
      this.onChangedCallback = onChangedCallback;
    }
  }
}

