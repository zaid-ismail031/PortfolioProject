using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using StarterAssets;

namespace DIGA7009A.Tools
{
  public class CameraSwitcher : MonoBehaviour
  {
    [SerializeField]
    private List<CinemachineVirtualCamera> VirtualCameras = new List<CinemachineVirtualCamera>();

    [SerializeField]
    private List<PlayerInput> PlayerInputs = new List<PlayerInput>();

    private int activeCameraIndex = 0;
    private const int ActivePriority = 10;
    private const int InactivePriority = 0;

    private void Start()
    {
      for( int i = 0; i < VirtualCameras.Count; i++ )
      {
        VirtualCameras[ i ].Priority = i == 0 ? ActivePriority : InactivePriority;
      }

      for( int i = 0; i < PlayerInputs.Count; i++ )
      {
        PlayerInputs[ i ].enabled = i == 0;
      }
      int next = ( activeCameraIndex ) % VirtualCameras.Count;
      SwitchCamera( next );
    }

    private void Update()
    {
      if( Input.GetKeyDown( KeyCode.Tab ) )
      {
        SwitchToNext();
      }
    }

    private void OnGUI()
    {
      float width = 250.0f;
      float height = 30.0f + ( VirtualCameras.Count * 35.0f );
      GUI.Box( new Rect( 10.0f, 10.0f, width, height ), "Camera Selection" );

      for( int i = 0; i < VirtualCameras.Count; i++ )
      {
        string label = VirtualCameras[ i ].name;
        if( i == activeCameraIndex )
        {
          label = "* " + label;
        }

        if( GUI.Button( new Rect( 20, 40 + i * 35f, width - 20, 30 ), label ) )
        {
          SwitchCamera( i );
        }
      }
    }

    public CinemachineVirtualCamera GetActiveVirtualCamera()
    {
      return VirtualCameras[ activeCameraIndex ];
    }

    public int GetActiveCameraIndex()
    {
      return activeCameraIndex;
    }

    private void SwitchToNext()
    {
      int next = ( activeCameraIndex + 1 ) % VirtualCameras.Count;
      SwitchCamera( next );
    }

    private void SwitchCamera( int index )
    {
      VirtualCameras[ activeCameraIndex ].Priority = InactivePriority;
      if( activeCameraIndex < PlayerInputs.Count )
      {
        PlayerInputs[ activeCameraIndex ].enabled = false;
      }

      activeCameraIndex = index;

      VirtualCameras[ activeCameraIndex ].Priority = ActivePriority;
      if( activeCameraIndex < PlayerInputs.Count )
      {
        PlayerInputs[ activeCameraIndex ].enabled = true;
      }
    }
  }
}
