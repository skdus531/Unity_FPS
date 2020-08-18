using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    //Weapon Specification
    public string weaponName;
    public int bulletsPerMag;
    public int bulletsTotal;
    public int currentBullets;
    public float range;
    public float fireRate;
    public float accuracy;
    private float originalAccuracy;
    public Vector3 aimPosition;
    private Vector3 originalPosition;
    public float damage;


    //Parameters
    private float fireTimer;
    private bool isReloading;
    private bool isAiming;
    private bool isRunning;

    //Recoil
    public Transform camRecoil;
    public Vector3 recoilKickback;
    public float recoilAmount;
    private float originalRecoil;


    //References
    public Transform shootPoint;
    public Transform bulletCasingPoint;
    private Animator anim;
    public ParticleSystem muzzleFlash;
    public Text bulletsText;
    private CharacterController characterController;

    //Prefabs
    public GameObject hitSparkPrefab;
    public GameObject hitHolePrefab;
    public GameObject bulletCasing;

    //Sounds
    public AudioSource audioSource;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioClip drawSound;

    private void Awake(){
        anim = GetComponent<Animator>();        
    }

        
    private void OnEnable() {
        anim.CrossFadeInFixedTime("Draw",0.01f);
        audioSource.clip = drawSound;
        audioSource.Play();
        bulletsText.text = currentBullets+" / "+bulletsTotal;
    }


    // Start is called before the first frame update
    private void Start()
    {    
        originalPosition = transform.localPosition;
        currentBullets = bulletsPerMag;
        bulletsText.text = currentBullets + " / " + bulletsTotal;
        originalAccuracy = accuracy;
        originalRecoil = recoilAmount;
        characterController = GetComponentInParent<CharacterController>();
    }

    // Update is called once per frame
    private void Update()
    {
        AimDownSights();
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        isReloading = info.IsName("Reload");

        if(Input.GetButton("Fire1")){
            if(currentBullets > 0){
                Fire();
            }else{
                DoReload();
            }
        }
        RecoilBack();
        if(Input.GetKeyDown(KeyCode.R)){
            DoReload();
        }
        Run();
        if(fireTimer < fireRate){
            fireTimer += Time.deltaTime;
        }
    }

    private void Fire(){
        if(fireTimer < fireRate || isReloading || isRunning ) return;
        RaycastHit hit;
        if(Physics.Raycast(shootPoint.position,shootPoint.transform.forward + Random.onUnitSphere * accuracy,out hit, range)){

            GameObject hitSpark = Instantiate(hitSparkPrefab, hit.point, Quaternion.FromToRotation(Vector3.up,hit.normal));
            hitSpark.transform.SetParent(hit.transform);
            Destroy(hitSpark,0.5f);
            GameObject hitHole = Instantiate(hitHolePrefab, hit.point, Quaternion.FromToRotation(Vector3.up,hit.normal));
            hitHole.transform.SetParent(hit.transform);
            Destroy(hitHole,5.0f);
            Recoil();
            BulletEffect();
            HealthManager healthManager = hit.transform.GetComponent<HealthManager>();
            if(healthManager) healthManager.ApplyDamage(damage);

            EnemyManager enemyManager = hit.transform.GetComponentInParent<EnemyManager>();
            if(enemyManager) enemyManager.ApplyDamage(damage);
            
            Rigidbody rigidbody = hit.transform.GetComponent<Rigidbody>();
            if(rigidbody){
                rigidbody.AddForceAtPosition(transform.forward * 5.0f * damage, transform.position);
            }
        }
        currentBullets--;
        fireTimer = 0.0f;
        audioSource.PlayOneShot(shootSound);
        anim.CrossFadeInFixedTime("Fire",0.01f);
        muzzleFlash.Play();
        bulletsText.text = currentBullets + " / " + bulletsTotal; 
    }

    private void DoReload(){
        if(!isReloading && currentBullets < bulletsPerMag && bulletsTotal > 0){
            anim.CrossFadeInFixedTime("Reload",0.01f); //reloading
                audioSource.clip = reloadSound;
                audioSource.Play();
        }
    }

    public void Reload(){
        int bulletsToReload = bulletsPerMag - currentBullets;
        if(bulletsToReload > bulletsTotal){
            bulletsToReload = bulletsTotal;
        }   
        currentBullets += bulletsToReload;
        bulletsTotal -= bulletsToReload;
        bulletsText.text = currentBullets + " / "+ bulletsTotal;

    }

    private void AimDownSights(){
        if(Input.GetButton("Fire2") && !isReloading){
            transform.localPosition = Vector3.Lerp(transform.localPosition,aimPosition,Time.deltaTime*8.0f);
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView,40.0f,Time.deltaTime*8.0f);
            accuracy = originalAccuracy / 1.5f;
            recoilAmount = originalRecoil / 1.5f;
            isAiming = true;
        }
        else{
            transform.localPosition = Vector3.Lerp(transform.localPosition,originalPosition,Time.deltaTime*5.0f);
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView,60.0f,Time.deltaTime*8.0f);
            accuracy = originalAccuracy;
            recoilAmount = originalRecoil;
            isAiming = false;
        }
    }

    private void Recoil(){
        Vector3 recoilVector = new Vector3(Random.Range(-recoilKickback.x,recoilKickback.x),recoilKickback.y,recoilKickback.z);
        Vector3 recoilCamVector = new Vector3(-recoilVector.y * 400.0f, recoilVector.x * 200.0f,0);

        transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition+recoilVector,recoilAmount/2.0f);
        camRecoil.localRotation = Quaternion.Slerp(camRecoil.localRotation,Quaternion.Euler(camRecoil.localEulerAngles+recoilCamVector),recoilAmount);

    }

    private void RecoilBack(){
        camRecoil.localRotation = Quaternion.Slerp(camRecoil.localRotation,Quaternion.identity,Time.deltaTime*2.0f);
    }

    private void BulletEffect(){
        Quaternion randomQuternion = new Quaternion (Random.Range(0,360),Random.Range(0,360),Random.Range(0,360),1);
        GameObject casing = Instantiate(bulletCasing,bulletCasingPoint);
        casing.transform.localRotation = randomQuternion;
        casing.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(Random.Range(50,100),Random.Range(50,100),Random.Range(-50,50)));
        Destroy(casing,1.0f);

    }

    private void Run(){
        anim.SetBool("isRunning",Input.GetKey(KeyCode.LeftShift));
        isRunning = characterController.velocity.sqrMagnitude > 99 ? true : false;
        anim.SetFloat("Speed",characterController.velocity.sqrMagnitude);
    }
}
