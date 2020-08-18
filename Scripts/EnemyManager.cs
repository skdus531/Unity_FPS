using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public float hitPoint = 100.0f;
    

    private Animator animator;
    private float timer = 0f;
    private float interval = 1.5f;
    private void Start(){
        animator = gameObject.GetComponent<Animator>();
    }

    private void Update() {
        timer += Time.deltaTime;
        float temp = Time.time * 100.0f;
        Random.InitState((int)temp);
        if(timer >= interval){
            animator.SetFloat("Horizontal",Random.Range(-1.0f,1.0f));
            animator.SetFloat("Vertical",Random.Range(-1.0f,1.0f));
            timer = 0.0f;
        }

        

    }
    public void ApplyDamage(float damage){
        hitPoint -= damage;
        if (hitPoint <=0){
            animator.enabled = false;
            Destroy(gameObject,10.0f);
        }
    }
}
