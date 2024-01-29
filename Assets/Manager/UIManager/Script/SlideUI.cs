using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideUI : MonoBehaviour
{
    // Start is called before the first frame update
    public int state = 0;
    public bool loop = false;

    public Vector3 outPos1;
    public Vector3 outPos2;
    public Vector3 inPos;
    // Update is called once per frame
    void Update()
    {
        if(state == 0)
        {
            if(transform.localPosition != outPos1)
            {
                transform.localPosition = outPos1;
            }
        }
        else if(state == 1)
        {
            Vector3 position = transform.localPosition;
            if(position.x > inPos.x - 1.0f && position.y > inPos.y - 1.0f && position.z > inPos.z - 1.0f)
            {
                transform.localPosition = inPos;
            }
            else
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition,inPos,4.0f*Time.unscaledDeltaTime);
            }
        }
        else if(state == 2)
        {
            if(transform.localPosition != outPos2)
            {
                Vector3 position = transform.localPosition;
                if(position.x > outPos2.x - 1.0f && position.y > outPos2.y - 1.0f && position.z > outPos2.z - 1.0f)
                {
                    transform.localPosition = outPos2;
                }
                else
                {
                    transform.localPosition = Vector3.Lerp(transform.localPosition,outPos2,2.0f*Time.unscaledDeltaTime);
                }
            }
            else
            {
                if(loop)
                {
                    state = 0;
                }
            }
        }
    }
}
