using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace CarLotJam.UIModule
{
    public class CoinSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject coinPrefab;
        private List<Transform> coins = new List<Transform>();
        private int _coinAmount;

        public void CreateCoins(int coinCount)
        {
            _coinAmount = coinCount;

            for (int i = 0; i < _coinAmount; i++)
            {
                Transform coinTransform = Instantiate(coinPrefab, transform).transform;
                coinTransform.GetComponent<RectTransform>()
                    .DOAnchorPos(new Vector2(Random.Range(-150, 150), Random.Range(-150, 150)), 0.2f);
                coins.Add(coinTransform);
            }

            CountCoins();
        }


        public void CountCoins()
        {
            var delay = 0f;

            for (int i = 0; i < coins.Count; i++)
            {
                coins[i].transform.DOScale(1f, 0.3f).SetDelay(delay).SetEase(Ease.OutBack);

                coins[i].transform.GetComponent<RectTransform>().DOAnchorPos(new Vector2(400f, 840f), 0.8f)
                    .SetDelay(delay + 0.5f).SetEase(Ease.InBack);


                coins[i].transform.DORotate(Vector3.zero, 0.5f).SetDelay(delay + 0.5f)
                    .SetEase(Ease.Flash);


                coins[i].transform.DOScale(0f, 0.3f).SetDelay(delay + 1.3f)
                    .SetEase(Ease.OutBack)
                    .OnComplete((() => Destroy(coins[i].gameObject)));

                delay += 0.1f;

               // counter.transform.parent.GetChild(0).transform.DOScale(1.1f, 0.1f).SetLoops(10, LoopType.Yoyo).SetEase(Ease.InOutSine).SetDelay(1.2f);
            }
            coins.Clear();
            StartCoroutine(CountDollars());
        }

        IEnumerator CountDollars()
        {
            yield return new WaitForSecondsRealtime(0.5f);
            PlayerPrefs.SetInt("CountDollar", PlayerPrefs.GetInt("CountDollar") + 50 + PlayerPrefs.GetInt("BPrize"));
            //counter.text = PlayerPrefs.GetInt("CountDollar").ToString();
            PlayerPrefs.SetInt("BPrize", 0);
        }
    }
}
