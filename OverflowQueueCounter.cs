using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverflowQueueCounter : Counter
{
    #region Overrides
    public override void Decrement()
    {
        this.count--;
    }
    public override void Increment()
    {
        this.count++;
    }
    public override void UpdateUI()
    {
        this.DisplayedText.text = this.baseText.ToString() + this.count.ToString();
    }
    #endregion
}
