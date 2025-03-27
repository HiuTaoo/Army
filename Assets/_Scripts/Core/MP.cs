using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MP : MonoBehaviour
{
    [SerializeField] private float maxMP;
    public float currentMP { get; private set; }
    private Coroutine mpRegenerationCoroutine;

    private void Awake()
    {
        currentMP = maxMP;
        StartCoroutine(RecoverMP());
    }
    public float getMaxMP()
    {
        return maxMP;
    }

    public void MPConsumpt(float _cost)
    {
        currentMP = Mathf.Clamp(currentMP - _cost, 0, maxMP);
    }

    public void RecoverMP(float _cost)
    {
        currentMP = Mathf.Clamp(currentMP + _cost, 0, maxMP);
    }

    // Hồi mana theo giây
    public void RecoverManaOverTime(float mpPerSecond, float duration)
    {
        if (mpRegenerationCoroutine != null)
        {
            StopCoroutine(mpRegenerationCoroutine); // Dừng lại việc hồi mana trước đó
        }
        mpRegenerationCoroutine = StartCoroutine(RecoverManaOverTimeCoroutine(mpPerSecond, duration));
    }

    private IEnumerator RecoverManaOverTimeCoroutine(float mpPerSecond, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            RecoverMP(mpPerSecond); // Hồi mana mỗi giây
            elapsedTime += 1f; // Cập nhật thời gian mỗi giây
            yield return new WaitForSeconds(1f); // Đợi 1 giây
        }
    }

    private IEnumerator RecoverMP()
    {
        while (true) 
        {
            if (currentMP < maxMP)
            {
                currentMP += 1; 
                currentMP = Mathf.Min(currentMP, maxMP); 
            }

            yield return new WaitForSeconds(20f); 
        }
    }



    public bool isEnoughMP(float _cost)
    {
        if(_cost > currentMP)
            return false;
        return true;
    }
    public float GetCurrentMP()
    {
        return currentMP;
    }

    public void ResetMP(float _value)
    {
        currentMP = _value;
    }

    public void MPRespawn(float mp)
    {
        ResetMP(mp);
    }
    public void IncreaseMP(float _value)
    {
        maxMP += _value;
        currentMP += (float)_value;
    }
}
