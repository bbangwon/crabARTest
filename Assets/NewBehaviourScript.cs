using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gestures;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using Unity.Linq;

public class NewBehaviourScript : MonoBehaviour {

    private CMobile3dGestures m_gestures;
    bool isCatchPossible = false;
    public GameObject p;
    public Text t;

    Collider c;

    bool catched = false;
    

    // Use this for initialization
    void Start () {
        m_gestures = GetComponent<CMobile3dGestures>();
        m_gestures.HandleGesture += ProcessGesture;
        m_gestures.Begin();

        gameObject.OnTriggerEnterAsObservable()
            .Delay(System.TimeSpan.FromSeconds(0.5f))
            .TakeUntil(gameObject.OnTriggerExitAsObservable())
            .RepeatUntilDestroy(this)             
            .Subscribe(_ =>
            {
                c = _;
                _.GetComponent<MeshRenderer>().material.color = Color.red;
                isCatchPossible = true;
            });

        gameObject.OnTriggerExitAsObservable()
            .Delay(System.TimeSpan.FromSeconds(0.2f))
            .Subscribe(_ =>
            {                
                _.GetComponent<MeshRenderer>().material.color = Color.white;
                isCatchPossible = false;
            });
    }
	
    private void ProcessGesture(Gesture gesture)
    {
        if (catched)
            return;

        t.text = gesture.m_dirDevice.ToString();
        Vector3 v = CUtil.ClosestAxis(gesture.m_dirDevice);

        if (isCatchPossible && v == Vector3.back)
        {
            if(c)
            {
                c.gameObject.transform.parent = gameObject.Child("cubeCase").transform;
                c.gameObject.transform.position = gameObject.Child("cubeCase").transform.position;
                catched = true;

                Observable.Timer(System.TimeSpan.FromSeconds(5f))
                    .Subscribe(_ =>
                    {
                        c.gameObject.transform.parent = null;
                        c.GetComponent<Rigidbody>().velocity = Vector3.zero;
                        c.transform.position = new Vector3(0, -2.5f, 2f);
                        c = null;
                        catched = false;
                    });
            }
            p.SetActive(true);
            Invoke("endT", 3f);
        }
    }

    void endT()
    {
        p.SetActive(false);
    }
}
