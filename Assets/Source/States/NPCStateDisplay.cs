using Source.States;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCStateDisplay : MonoBehaviour
{
  
  //[SerializeField] private NPCController npcController;

  [SerializeField] private bool isHSM;
  
  [SerializeField] private TMP_Text stateText;
  
  [SerializeField] private HSMController hsmController;
  [SerializeField] private NPCStateController stateController;
  

  private Camera mainCamera;
  //private RectTransform fillRect;
  [SerializeField]private Transform canvasTransform;

  private void Start()
  {
    mainCamera = Camera.main;
    
  }
  
  private void LateUpdate()
  {
    if( !mainCamera || !hsmController || !stateController )
    {
      return;
    }
    
    //string state updates
    if (isHSM)
    {
       var s = hsmController.GetStateText();
       stateText.text =  s;
    }
    else
    {
      var s = stateController.GetStateText();
      stateText.text = s;
    }

    
    
    // Face the camera
    canvasTransform.forward = mainCamera.transform.forward;
  }
}
