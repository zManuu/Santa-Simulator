using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowSpawner : MonoBehaviour
{

    [SerializeField] private Transform _camera;
    [SerializeField] private Transform _livingFlakeContainer;
    [SerializeField] private GameObject[] _snowFlakes;

    private void Start()
    {
        StartCoroutine(StartSpawner());
    }

    private IEnumerator StartSpawner()
    {
        while (true)
        {
            int spawnIndex = Random.Range(0, _snowFlakes.Length - 1);
            GameObject snowFlake = Instantiate(_snowFlakes[spawnIndex], _livingFlakeContainer);
            snowFlake.SetActive(true);
            int posX = Random.Range(Mathf.RoundToInt(_camera.position.x - 10), Mathf.RoundToInt(_camera.position.x + 10));
            snowFlake.transform.position = new Vector3(posX, _camera.position.y + 10);
            Destroy(snowFlake, 5);
            yield return new WaitForSeconds(0.125f);
        }
    }

}
