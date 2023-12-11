using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultView : ViewBase
{
    [SerializeField] private ExtendButton toTitleButton;

    void Start()
    {

    }
    public override IEnumerator Wait()
    {
        while (true)
        {
            //ƒ^ƒCƒgƒ‹‚É–ß‚é
            if (toTitleButton.IsClicked)
            {
                yield break;
            }

            yield return null;
        }
    }
}