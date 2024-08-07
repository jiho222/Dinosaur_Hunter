using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Animator anim;
    private Rigidbody rigid;
    float hAxis;
    float vAxis;
    bool rDown;
    bool jDown;
    bool fDown;
    bool isRun;
    bool isJump;
    bool isAttackReady;
    bool isDamage;
    bool isDead;
    Vector3 moveVec;
    public float speed = 5.0f;
    public float runSpeed = 8.0f;
    public float jumpPower = 10.0f;
    private float finalSpeed;
    private float rotationSpeed = 720.0f; // 초당 회전 각도
    GameObject nearObject;
    public int health;
    public int maxHealth;
    float attackDelay;
    Weapon myWeapon;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        myWeapon = GetComponentInChildren<Weapon>();
    }

    void Update()
    {
        GetInput();
        Move();
        Run();
        Jump();
        Attack();
    }

    void GetInput()
    {
        hAxis = Input.GetAxis("Horizontal");
        vAxis = Input.GetAxis("Vertical");
        rDown = Input.GetKey(KeyCode.LeftShift);
        jDown = Input.GetKeyDown(KeyCode.Space);
        fDown = Input.GetKeyDown(KeyCode.Mouse0);
    }

    void Move()
    {
        if (isDead)
        {
            return;
        }
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();
        moveVec = (forward * vAxis + right * hAxis).normalized;
        transform.position += moveVec * finalSpeed * Time.deltaTime;

        if (moveVec != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveVec, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void Run()
    {
        if (isDead)
        {
            return;
        }        
        if (rDown)
        {
            isRun = true;
        }
        else
        {
            isRun = false;
        }
        finalSpeed = (isRun) ? runSpeed : speed; // 달리기 상태면 runSpeed, 아니면 speed 사용
        Vector3 moveDirection = new Vector3(hAxis, 0.0f, vAxis).normalized;
        float percent = ((isRun) ? 1 : 0.5f) * moveDirection.magnitude; // 달리기 상태면 1, 아니면 0.5를 곱하여 percent 변수에 저장
        anim.SetFloat("Blend", percent, 0.1f, Time.deltaTime);
    }

    void Jump()
    {
        if (isDead)
        {
            return;
        }
        if (jDown && !isJump)
        {
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            isJump = true;
        }
    }

    void Attack()
    {   
        if (isDead)
        {
            return;
        }
        attackDelay += Time.deltaTime;
        isAttackReady = myWeapon.rate < attackDelay;
        if (fDown && isAttackReady) {
            myWeapon.Use();
            anim.SetTrigger("doSwing");
            attackDelay = 0;
        }
    }

    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero;
    }

    void FixedUpdate()
    {
        FreezeRotation();
    }   

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isJump = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item") {
            Item item = other.GetComponent<Item>();
            health += item.value;
            if (health > maxHealth) {
                health = maxHealth; }
            Destroy(other.gameObject);
        }
        else if (other.tag == "EnemyMelee") {
            if (!isDamage)
            {
                EnemyMelee enemyMelee = other.GetComponent<EnemyMelee>();
                health -= enemyMelee.damage;
                StartCoroutine(OnDamage());
                if (health <= 0)
                {
                    OnDie();
                }
            }
        }
    }
    
    IEnumerator OnDamage()
    {
        isDamage = true;
        yield return new WaitForSeconds(1.0f);
        isDamage = false;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Item")
        {
            nearObject = other.gameObject;
        }
    }

    void OnDie()
    {
        anim.SetTrigger("isDie");
        isDead = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Item")
        {
            nearObject = null;
        }
    }
}