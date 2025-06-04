using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData
{
    public string FileName { get; private set; }
    public int index { get; set; }

    protected SaveData()
    {
        FileName = $"{this.GetType().ToString()}_{index}";
    }
}
