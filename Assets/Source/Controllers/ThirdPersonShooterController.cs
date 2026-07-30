using Cinemachine;
using StarterAssets;
using UnityEngine;
using UnityEngine.UI;

namespace DIGA7009A
{
  public class ThirdPersonShooterController : MonoBehaviour
  {
    [Header( "Camera" )]
    [SerializeField]
    private CinemachineVirtualCamera AimVirtualCamera;

    [Header( "Camera Sensitivity" )]
    [SerializeField]
    private float WalkAroundSensitivity = 1.0f;
    [SerializeField]
    private float AimSensitivity = 0.5f;

    [Header( "CrossHair" )]
    [SerializeField]
    private RawImage CrossHair;
    [SerializeField]
    private LayerMask AimColliderMask = new LayerMask();

    [Header( "Bullet" )]
    [SerializeField]
    private GameObject BulletProjectile;
    [SerializeField]
    private Transform SpawnBulletPosition;

    private Animator animator;
    private StarterAssetsInputs starterAssetsInputs;
    private ThirdPersonController thirdPersonController;
    private Vector3 mouseWorldPosition;
    private Transform hitScanTransform;

    // Start is called before the first frame update
    private void Start()
    {
      starterAssetsInputs = GetComponent<StarterAssetsInputs>();
      thirdPersonController = GetComponent<ThirdPersonController>();
      animator = GetComponent<Animator>();
    }

    private void Update()
    {
      SwitchCameraModes();
      AimPosition();
      SetAimAnimation();
      ShootProjectile();
      HitScan();
    }

    private void SwitchCameraModes()
    {
      if( starterAssetsInputs.aim )
      {
        OverTheShoulderCam();
        CrossHairVisibility( true );
        SetPlayerRotation();
      }
      else
      {
        WalkAroundCam();
        CrossHairVisibility( false );
      }
    }

    private void OverTheShoulderCam()
    {
      AimVirtualCamera.gameObject.SetActive( true );
      thirdPersonController.SetCameraSensitivity( AimSensitivity );
      thirdPersonController.ToggleRotateOnMove( false );
    }

    private void WalkAroundCam()
    {
      AimVirtualCamera.gameObject.SetActive( false );
      thirdPersonController.SetCameraSensitivity( WalkAroundSensitivity );
      thirdPersonController.ToggleRotateOnMove( true );
    }

    private void CrossHairVisibility( bool show )
    {
      CrossHair.gameObject.SetActive( show );
    }

    private void AimPosition()
    {
      mouseWorldPosition = Vector3.zero;
      hitScanTransform = null;

      Vector2 screenCenterPoint = new Vector2( Screen.width / 2.0f, Screen.height / 2.0f );
      Ray ray = Camera.main.ScreenPointToRay( screenCenterPoint );

      if( Physics.Raycast( ray, out RaycastHit hit, 999.0f, AimColliderMask ) )
      {
        mouseWorldPosition = hit.point;
        hitScanTransform = hit.transform;
      }
      else
      {
        mouseWorldPosition = ray.GetPoint( 20.0f );
      }
    }

    private void SetPlayerRotation()
    {
      Vector3 worldAimTarget = mouseWorldPosition;
      worldAimTarget.y = transform.position.y;
      Vector3 aimDirection = ( worldAimTarget - transform.position ).normalized;

      transform.forward = Vector3.Lerp( transform.forward, aimDirection, Time.deltaTime * 20.0f );
    }

    private void ShootProjectile()
    {
      if( starterAssetsInputs.aim && starterAssetsInputs.fire )
      {
        Vector3 aimDirection = ( mouseWorldPosition - SpawnBulletPosition.position ).normalized;
        GameObject newBullet = Instantiate( BulletProjectile, SpawnBulletPosition.position, Quaternion.LookRotation( aimDirection, Vector3.up ) );
        BulletProjectile bulletProjectile = newBullet.GetComponent<BulletProjectile>();
        bulletProjectile.SetBulletSpeed( 5.0f );
        bulletProjectile.ToggleDestroyOnCollision( true );
        starterAssetsInputs.fire = false;
      }
    }

    private void HitScan()
    {
      if( starterAssetsInputs.aim && starterAssetsInputs.fire )
      {
        if( hitScanTransform?.GetComponent<BulletTarget>() != null )
        {
          // Hit target
        }
        else
        {
          // Hit something else
        }
      }
    }

    private void SetAimAnimation()
    {
      if( starterAssetsInputs.aim )
      {
        float animationWeightInterpolate = Mathf.Lerp( animator.GetLayerWeight( 1 ), 1.0f, Time.deltaTime * 10.0f );
        animator.SetLayerWeight( 1, animationWeightInterpolate );
      }
      else
      {
        float animationWeightInterpolate = Mathf.Lerp( animator.GetLayerWeight( 1 ), 0.0f, Time.deltaTime * 20.0f );
        animator.SetLayerWeight( 1, animationWeightInterpolate );
      }
    }
  }
}

