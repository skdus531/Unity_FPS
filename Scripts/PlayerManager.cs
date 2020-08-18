using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    private float hitPoint = 100.0f;
    public float stamina = 1000.0f;
    private float maxHitPoint;
    private float maxStamina;
    public Text hPText;
    public Text stText;
    public RectTransform hPBar;
    public RectTransform stBar;
    private CharacterController characterController;

    // Start is called before the first frame update
    private void Start()
    {
        maxHitPoint = hitPoint;
        maxStamina = stamina;
        hPText.text = hitPoint + " / " + maxHitPoint;
        stText.text = ((int)stamina/maxStamina*100.0f).ToString() + "%";
        hPBar.localScale = Vector3.one;
        stBar.localScale = Vector3.one;
        characterController = GetComponentInParent<CharacterController>();
    }

    // Update is called once per frame
    private void Update()
    {

        if(characterController.velocity.sqrMagnitude>99 && Input.GetKey(KeyCode.LeftShift) && stamina>0){
            stamina-=0.2f;
            UpdateST();
        }
        else if(stamina < maxStamina){
            stamina += 0.1f;
            UpdateST();
        }
        
        if(Input.GetKeyDown(KeyCode.K)){
            ApplyDamage(Random.Range(1,20));
        }

    }

    public void ApplyDamage(float damage){
        UpdateHP();
        hitPoint -= damage;
        if (hitPoint <= 0) {
            hitPoint = 0;
            Debug.Log("You died!");
        }
    }

    private void UpdateHP(){
        hPText.text = hitPoint + " / " + maxHitPoint;
        hPBar.localScale = new Vector3(hitPoint/maxHitPoint,1,1);
    }

    private void UpdateST(){
        stText.text = ((int)stamina/maxStamina*100.0f).ToString() + "%";
        stBar.localScale = new Vector3(stamina/maxStamina,1,1);
    }

}
