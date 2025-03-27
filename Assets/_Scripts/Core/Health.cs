using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private Behaviour[] components;
    private Animator anim;
    private Coroutine healingCoroutine; // Coroutine để hồi máu theo giây
    public float currentHealth { get; private set; }
    private Boolean isDead = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    public void takeDamage(float _damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, maxHealth);
        if (currentHealth > 0)
        {
            anim.SetTrigger("hurt");
        }
        else
        {
            anim.SetTrigger("dead");
            isDead = true;
            
        }
    }
    public void Healing(float hp)
    {
        currentHealth = Mathf.Clamp(currentHealth + hp, 0, maxHealth);
    }

    // Hồi máu theo giây
    public void HealOverTime(float hpPerSecond, float duration)
    {
        if (healingCoroutine != null)
        {
            StopCoroutine(healingCoroutine); // Dừng hồi máu nếu có coroutine hồi máu trước đó
        }
        healingCoroutine = StartCoroutine(HealOverTimeCoroutine(hpPerSecond, duration));
    }

    private IEnumerator HealOverTimeCoroutine(float hpPerSecond, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            Healing(hpPerSecond); // Hồi máu mỗi lần
            elapsedTime += 1f; // Cập nhật thời gian mỗi giây
            yield return new WaitForSeconds(1f); // Đợi 1 giây trước khi hồi máu tiếp
        }
    }

    public void AddHealth(float _value)
    {
        currentHealth = _value;
    }

    public float getMaxHealth()
    {
        return maxHealth;
    }

    public void EnableMovement()
    {
        GetComponent<PlayerMovement>().enabled = true;
    }

    public void DeActive()
    {
        gameObject.SetActive(false);
    }

    public void HealthRespawn(float health)
    {
        AddHealth(health);
        foreach (Behaviour comp in components)
            comp.enabled = true;
        anim.ResetTrigger("dead");
        anim.Play("idle");
        StartCoroutine(Invunerability());
        isDead = false;

        
    }

    private IEnumerator Invunerability()
    {
        yield return new WaitForSeconds(2f);
    }

    public void SuddenDeath()
    {
        takeDamage(maxHealth);
    }

    public float getCurrentHealth()
    {
        return currentHealth;
    }

    public void Heal()
    {
        currentHealth = maxHealth;
    }
    public void IncreaseHP(float _value)
    {
        maxHealth += _value;
        currentHealth += (float)_value;
    }

    public Boolean IsDead()
    {
        return isDead;
    }
}
