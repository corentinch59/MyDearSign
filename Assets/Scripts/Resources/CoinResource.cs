using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class CoinResource : MonoBehaviour, IPannalInteractable
{
    [Header("Config")] 
    [SerializeField] private int _coinGeneration;
    [SerializeField] private float _cooldown;

    [Header("Chest Tween Config")]
    [SerializeField] private Vector3 _scale;
    [SerializeField] private Vector3 _tweenConfig;

    [Header("Coin Tween Config")] 
    [SerializeField] private Transform _coin;
    [SerializeField] private Transform _coinTarget;
    [SerializeField] private Vector3 _coinJumpConfig;
    [SerializeField] private Vector3 _coinRotation;

    public UnityEvent bonusEnableFunction;
    public UnityEvent bonusDisableFunction;
    public UnityEvent resourceTriggered;

    private Coroutine _moneyGenerationCoroutine;

    public void EnableResource(int batteryLevel)
    {
        _moneyGenerationCoroutine = StartCoroutine(GenerateMoney());
        bonusEnableFunction?.Invoke();
    }

    public void DisableResource()
    {
        if (_moneyGenerationCoroutine != null) StopCoroutine(_moneyGenerationCoroutine);
        bonusDisableFunction?.Invoke();
    }

    public void RandomPitch(AudioSource source)
    {
        source.pitch = Random.Range(0.9f, 1.1f);
    }

    IEnumerator GenerateMoney()
    {
        while (true)
        {
            transform.DOPunchScale(_scale, _tweenConfig.x, (int)_tweenConfig.y, _tweenConfig.z);

            Sequence s = DOTween.Sequence();
            _coin.DORotate(new Vector3(0, 360, 0), 1f, RotateMode.FastBeyond360).SetRelative(true)
                .SetEase(Ease.Linear).SetLoops(-1);

            s.Append(_coin.DOJump(_coinTarget.position, _coinJumpConfig.x, (int)_coinJumpConfig.y, _coinJumpConfig.z));
            s.Append(_coin.DOScale(Vector3.zero, .75f));
            s.onComplete += () => {
                _coin.localPosition = new Vector3(0, -0.057f, 0);
                _coin.localScale = new Vector3(1, 1, 1);
            };

            GameManager.instance.money += _coinGeneration;
            resourceTriggered?.Invoke();

            yield return new WaitForSeconds(_cooldown);
        }
    }
}
