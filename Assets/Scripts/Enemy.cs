using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type { A, B, C };
    public Type enemytype;
    public int maxHealth;
    public int curHealth;
    public Transform target;
    public BoxCollider meleeArea;
    public bool isChase;
    public bool isAttack;
    public GameObject chicken;

    Rigidbody rigid;
    CapsuleCollider capsuleCollider;
    Weapon myWeapon;
    NavMeshAgent nav;
    Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        Invoke("ChaseStart", 2);
    }

    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isMove", true);
    }

    void Update()
    {
        if (nav.enabled)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }
    }

    void FreezeVelocity()
    {   
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
    }

    void Targeting()
    {
        float targetRadius = 0;
        float targetRange = 0;

        switch (enemytype)
        {
            case Type.A:
                targetRadius = 1.5f;
                targetRange = 5;
                break;
            case Type.B:
                targetRadius = 1.0f;
                targetRange = 15;
                break;
            case Type.C:
                targetRadius = 1.5f;
                targetRange = 15;
                break;
        }


        RaycastHit[] rayHits = 
            Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));

        if (rayHits.Length > 0 && ! isAttack)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);

        switch (enemytype)
        {
            case Type.A:
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;
        
                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false;
        
                yield return new WaitForSeconds(1f);
                isChase = true;
                isAttack = false;
                anim.SetBool("isAttack", false);
                break; // Add break statement here
        
            case Type.B:
                yield return new WaitForSeconds(1.0f);
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero;
                meleeArea.enabled = false;

                yield return new WaitForSeconds(2.0f);
                break;
            case Type.C:
                yield return new WaitForSeconds(2.0f);
                break;
        }
    }

    void FixedUpdate()
    {
        Targeting();
        FreezeVelocity();
    }   

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            Weapon myWeapon = other.GetComponent<Weapon>();
            curHealth -= myWeapon.damage;

            // 데미지가 0 이하가 되면 죽음
            if (curHealth <= 0)
            {
                isChase = false;
                nav.enabled = false;
                anim.SetTrigger("isDie");
                gameObject.layer = 9;
                StartCoroutine(DestroyAndActivateChicken());
            }
        }
    }

    IEnumerator DestroyAndActivateChicken()
    {
        GameObject chicken = transform.Find("Chicken").gameObject; // 치킨 아이템 찾기
        yield return new WaitForSeconds(4); // 4초 기다림
        chicken.transform.SetParent(null); // 치킨 아이템의 부모를 null로 설정하여 분리
        chicken.SetActive(true); // 치킨 아이템 활성화
        yield return null; // 한 프레임 대기하여 치킨 활성화가 반영되도록 함
        Destroy(gameObject); // 현재 오브젝트 삭제
    }
}
