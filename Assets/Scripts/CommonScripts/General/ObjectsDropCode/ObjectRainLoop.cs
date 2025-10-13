using UnityEngine;
using DG.Tweening;
//Bu kod objelerin yukarıdan aşağı düşmesini saglayan koddur.
public class ObjectRainLoop : MonoBehaviour
{
    public GameObject[] objectPrefabs;
    public Transform dropObjParent;
    public float spawnInterval = 0.2f;
    public float fallDuration = 2f;
    public float fallHeight = 5f;
    public float horizontalRange = 8f; // saga sola yayilma

    private void OnEnable()
    {
        StartCoroutine(SpawnLoop());
    }

    private System.Collections.IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnFallingObject();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnFallingObject()
    {
        float randomX = Random.Range(-horizontalRange, horizontalRange);
        float startY = Camera.main.transform.position.y + fallHeight;
        float targetY = Camera.main.transform.position.y - 6f;

        var prefab = objectPrefabs[Random.Range(0, objectPrefabs.Length)];
        if (prefab == null) return;

        GameObject obj = Instantiate(prefab, new Vector3(randomX, startY, 0), Quaternion.identity, dropObjParent);

        var resetter = obj.AddComponent<ObjectResetter>();
        resetter.initialLocalPos = obj.transform.localPosition;

        // Hafif X kayması için hedef pozisyon belirle
        float xOffset = Random.Range(-1f, 1f);
        Vector3 targetPos = new Vector3(randomX + xOffset, targetY, 0);

        // Yumuşak düşüş (X + Y)
        obj.transform.DOLocalMove(targetPos, fallDuration)
            .SetEase(Ease.InOutSine);

        // Belirli sürede yok et
        DOVirtual.DelayedCall(fallDuration + 0.2f, () =>
        {
            DOTween.Kill(obj.transform);
            obj.SetActive(false);
        });
    }


}
