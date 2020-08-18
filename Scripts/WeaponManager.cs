using System.Collections;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    // Start is called before the first frame update

    public float switchDelay = 1.0f;
    public GameObject[] weapon;

    private int idx = 0;
    private bool isSwitching = false;

    private void Start()
    {
        InitializeWeapon();
    }

    // Update is called once per frame
    private void Update()
    {
        if(Input.GetAxis("Mouse ScrollWheel")>0 && !isSwitching){
            idx++;
            if(idx>=weapon.Length) idx = 0;
            StartCoroutine(SwitchDelay(idx));
        }
        if(Input.GetAxis("Mouse ScrollWheel")<0 && !isSwitching){
            idx--;
            if(idx<0) idx = weapon.Length-1;
            StartCoroutine(SwitchDelay(idx));
        }

        for(int i = 49; i<58; i++){
            if(Input.GetKeyDown((KeyCode)i) && !isSwitching && weapon.Length > i-49 && idx != i-49){
                idx = i-49;
                StartCoroutine(SwitchDelay(idx));
            }
        }
    }

    private void InitializeWeapon(){
        for (int i = 0 ; i< weapon.Length; i++){
            weapon[i].SetActive(false);
        }
        weapon[0].SetActive(true);
        idx = 0;
    }

    private IEnumerator SwitchDelay(int new_idx){
        isSwitching = true;
        SwitchWeapons(new_idx);
        yield return new WaitForSeconds(switchDelay);
        isSwitching = false;
    }

    private void SwitchWeapons(int new_idx){
        for(int i = 0; i < weapon.Length; i++){
            weapon[i].SetActive(false);
        }
        weapon[new_idx].SetActive(true);

    }

}
