using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

[System.Serializable]
public class PipePath
{
    public GameObject[] pipeObjects;
}

public class PipeManager : MonoBehaviour
{
    [SerializeField] private string pipeTag = "Pipe";
    [SerializeField] private List<PipePath> paths;
    [FormerlySerializedAs("uiManager")] [SerializeField] UIController uiController;

    private Dictionary<Transform, float> correctRotations = new Dictionary<Transform, float>();
    private Dictionary<Transform, bool> isStraightPipe = new Dictionary<Transform, bool>();

    void Start()
    {
        GameObject[] pipes = GameObject.FindGameObjectsWithTag(pipeTag);

        foreach (GameObject pipe in pipes)
        {
            float correctZ = pipe.transform.eulerAngles.z;
            correctRotations[pipe.transform] = correctZ;

            Pipe pipeScript = pipe.GetComponent<Pipe>();
            bool isStraight = pipeScript != null && pipeScript.isStraight;
            isStraightPipe[pipe.transform] = isStraight;

            int[] angles = { 0, 90, 180, 270 };
            int randomAngle = angles[Random.Range(0, angles.Length)];
            pipe.transform.rotation = Quaternion.Euler(0, 0, randomAngle);
        }
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                Vector2 touchPos2D = new Vector2(touchPosition.x, touchPosition.y);

                RaycastHit2D hit = Physics2D.Raycast(touchPos2D, Vector2.zero);
                if (hit.collider != null && hit.collider.CompareTag(pipeTag))
                {
                    Transform pipe = hit.collider.transform;

                    float currentZ = Mathf.Round(pipe.eulerAngles.z / 90f) * 90f;
                    float newZ = (currentZ + 90f) % 360f;
                    pipe.rotation = Quaternion.Euler(0, 0, newZ);

                    StartCoroutine(AnimatePipeTouch(pipe)); // Play animation

                    if (CheckAllPathsCorrect())
                    {
                        Debug.Log("Game Win: All pipe paths are correctly rotated!");
                        uiController.GameWon();
                    }
                }

            }
        }
    }
    private IEnumerator AnimatePipeTouch(Transform pipe)
    {
        Vector3 originalScale = pipe.localScale;
        Vector3 targetScale = originalScale * 1.2f;
        float duration = 0.1f;

        // Scale up
        float time = 0f;
        while (time < duration)
        {
            pipe.localScale = Vector3.Lerp(originalScale, targetScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        pipe.localScale = targetScale;

        // Scale down
        time = 0f;
        while (time < duration)
        {
            pipe.localScale = Vector3.Lerp(targetScale, originalScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        pipe.localScale = originalScale;
    }


    bool CheckAllPathsCorrect()
    {
        foreach (PipePath path in paths)
        {
            if (!SinglePathCorrect(path.pipeObjects))
                return false;
        }
        return true;
    }

    bool SinglePathCorrect(GameObject[] pipeObjects)
    {
        foreach (GameObject pipeObj in pipeObjects)
        {
            if (pipeObj == null) continue;

            Transform pipe = pipeObj.transform;
            float currentZ = pipe.eulerAngles.z;
            float correctZ = correctRotations[pipe];
            bool isStraight = isStraightPipe[pipe];

            float delta = Mathf.Abs(Mathf.DeltaAngle(currentZ, correctZ));

            if (isStraight)
            {
                // Accept 0 or 180 for straight pipes
                if (delta > 1f && Mathf.Abs(delta - 180f) > 1f)
                    return false;
            }
            else
            {
                // Only exact match for non-straight pipes
                if (delta > 1f)
                    return false;
            }
        }

        return true;
    }
    public void SolvePuzzle()
    {
        StartCoroutine(SolveAllPipes());
    }

    private IEnumerator SolveAllPipes()
    {
        foreach (Transform pipe in correctRotations.Keys)
        {
            float correctZ = correctRotations[pipe];

            // For straight pipes, optionally choose 0 or 180 randomly if both are valid
            if (isStraightPipe[pipe])
            {
                correctZ = (Random.value > 0.5f) ? correctZ : (correctZ + 180f) % 360f;
            }

            pipe.rotation = Quaternion.Euler(0, 0, correctZ);
        }

        yield return new WaitForSeconds(1f);

        if (CheckAllPathsCorrect())
        {
            Debug.Log("Puzzle solved automatically. Game Win!");
            uiController.GameWon();
        }
    }

    
}
