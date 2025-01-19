using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Movement
{
    public static IEnumerator IEMove(Transform from, Transform to, float duration)
    {
        var runTime = 0.0f;
        Vector3 startPos = from.position;
        from.LookAt(to);
        while (runTime < duration)
        {
            runTime += Time.deltaTime / GameManager.Instance.gameSpeed;

            from.position = Vector3.Lerp(startPos, to.position, runTime / duration);

            yield return null;
        }
        from.position = Vector3.Lerp(startPos, to.position, 1);
    }
    public static IEnumerator IEMove(Transform from, Transform to, float duration, float lessDistance)
    {
        var runTime = 0.0f;
        Vector3 startPos = from.position;
        Vector3 direction = (to.position - from.position).normalized * lessDistance;
        Vector3 endPos = to.position - direction;
        from.LookAt(to);
        while (runTime < duration)
        {
            runTime += Time.deltaTime / GameManager.Instance.gameSpeed;

            from.position = Vector3.Lerp(startPos, endPos, runTime / duration);

            yield return null;
        }
        from.position = Vector3.Lerp(startPos, endPos, 1);
    }
    public static IEnumerator IERotate(Transform from, Transform to)
    {
        float time = 0;
        while (time < 1f)
        {
            from.rotation = Quaternion.Lerp(from.rotation, to.rotation, 1 / GameManager.Instance.gameSpeed * 0.05f);
            time += Time.deltaTime;
            yield return null;
        }
    }
}
