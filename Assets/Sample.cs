using System.Collections;
using System.Diagnostics;
using UnityEngine;

using MersenneTwister;
using PCGSharp;

using Debug = UnityEngine.Debug;

public class Sample : MonoBehaviour
{
    public static void AutoResize(int screenWidth, int screenHeight)
    {
        Vector2 resizeRatio = new Vector2((float)Screen.width / screenWidth, (float)Screen.height / screenHeight);
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(resizeRatio.x, resizeRatio.y, 1.0f));
    }

    public int count = 20000;
    public int bench = 1000000;

    private GameObject prefab;

    private Vector3 range;

    private Transform tr;
    private BoxCollider collider;

    private Stopwatch sw;
    private System.Random sys;
    private mt19937_64 mt;
    //private Well512 well;
    private Pcg pcg;

    void Awake()
    {
        prefab = Resources.Load<GameObject>("Dot");

        tr = gameObject.transform;
        collider = GetComponent<BoxCollider>();

        range = collider.size;

        sw = new Stopwatch();

        sys = new System.Random();
        mt = new mt19937_64();
        //well = new Well512();
        pcg = new Pcg();

        Well512.Next();
    }

    //void LateUpdate()
    //{
    //}

    void OnGUI()
    {
        AutoResize(480, 800);

        GUILayout.BeginHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginVertical();

        GUILayout.Label(new string('-', 30));

        if (GUILayout.Button("Clear"))
        {
            Clear();
        }

        if (GUILayout.Button("Unity Random"))
        {
            Clear();
            StartTest(GenUnityRandom, "Unity Random", count);
        }

        if (GUILayout.Button("System Random"))
        {
            Clear();
            StartTest(GenSystemRandom, "System Random", count);
        }

        if (GUILayout.Button("Mt Random"))
        {
            Clear();
            StartTest(GenMtRandom, "Mt Random", count);
        }

        if (GUILayout.Button("Well Random"))
        {
            Clear();
            StartTest(GenWellRandom, "Well Random", count);
        }

        if (GUILayout.Button("PCG Random"))
        {
            Clear();
            StartTest(GenPCGRandom, "PCG Random", count);
        }

        if (GUILayout.Button("Benchmark"))
        {
            StopCoroutine(StartBench());
            StartCoroutine(StartBench());
        }

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }

    private IEnumerator StartBench()
    {
        yield return new WaitForEndOfFrame();

        StartTest(() =>
        {
            var n = Random.Range(0.0f, 1.0f);
        }, "Unity Random", bench);
        yield return new WaitForEndOfFrame();

        StartTest(() =>
        {
            var n = sys.NextDouble();
        }, "System Random", bench);
        yield return new WaitForEndOfFrame();

        StartTest(() =>
        {
            var n = mt.genrand64_real1();
        }, "Mt Random 1", bench);
        yield return new WaitForEndOfFrame();

        StartTest(() =>
        {
            var n = mt.genrand64_real2();
        }, "Mt Random 2", bench);
        yield return new WaitForEndOfFrame();

        StartTest(() =>
        {
            var n = mt.genrand64_real3();
        }, "Mt Random 3", bench);
        yield return new WaitForEndOfFrame();

        StartTest(() =>
        {
            var n = Well512.Next();
        }, "Well Random", bench);
        yield return new WaitForEndOfFrame();

        StartTest(() =>
        {
            var n = pcg.NextDouble();
        }, "PCG Random", bench);
        yield return new WaitForEndOfFrame();
    }

    private void StartTest(System.Action action, string title, int repeat)
    {
        sw.Reset();
        sw.Start();

        for (int i = 0; i < repeat; i++)
        {
            action();
        }

        sw.Stop();

        Debug.Log(title + " : " + sw.ElapsedMilliseconds);
    }

    private void CreatePrefab(Vector3 pos)
    {
        Instantiate(prefab, pos, tr.rotation, tr);
    }

    private float GetValue(float v, float r)
    {
        return Mathf.Round(((int) v - (int) (r * 0.5f)) * 2.0f) * 0.5f;
    }

    public void Clear()
    {
        foreach (Transform child in tr)
        {
            Destroy(child.gameObject);
        }
    }

    public void GenUnityRandom()
    {
        Vector3 pos = new Vector3
        {
            x = GetValue(Random.Range(0, (int) range.x), range.x),
            y = GetValue(Random.Range(0, (int) range.y), range.y),
            z = GetValue(Random.Range(0, (int) range.z), range.z)
        };

        CreatePrefab(pos);
    }

    public void GenSystemRandom()
    {
        Vector3 pos = new Vector3
        {
            x = GetValue(sys.Next(0, (int) range.x), range.x),
            y = GetValue(sys.Next(0, (int) range.y), range.y),
            z = GetValue(sys.Next(0, (int) range.z), range.z)
        };

        CreatePrefab(pos);
    }

    public void GenMtRandom()
    {
        Vector3 pos = new Vector3
        {
            x = GetValue((int) (mt.genrand64_real1() * range.x), range.x),
            y = GetValue((int) (mt.genrand64_real1() * range.y), range.y),
            z = GetValue((int) (mt.genrand64_real1() * range.z), range.z)
        };

        CreatePrefab(pos);
    }

    public void GenWellRandom()
    {
        Vector3 pos = new Vector3
        {
            x = GetValue(Well512.Next(0, (int) range.x), range.x),
            y = GetValue(Well512.Next(0, (int) range.y), range.y),
            z = GetValue(Well512.Next(0, (int) range.z), range.z)
        };

        CreatePrefab(pos);
    }

    public void GenPCGRandom()
    {
        Vector3 pos = new Vector3
        {
            x = GetValue(pcg.Next((int) range.x), range.x),
            y = GetValue(pcg.Next((int) range.y), range.y),
            z = GetValue(pcg.Next((int) range.z), range.z)
        };

        CreatePrefab(pos);
    }
}
