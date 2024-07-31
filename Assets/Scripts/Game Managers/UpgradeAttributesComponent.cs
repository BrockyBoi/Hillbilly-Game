using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weaponry;

public class UpgradeAttributesComponent
{
    private Dictionary<UpgradeAttribute, float> _upgradeAttributes = new Dictionary<UpgradeAttribute, float>();

    public delegate void EOnAttributeChange(UpgradeAttribute attribute, float value);
    public event EOnAttributeChange OnAttributeChanged;

    public float GetAttribute(UpgradeAttribute attributeType)
    {
        if (!_upgradeAttributes.ContainsKey(attributeType))
        {
            float defaultValue = 0;
            if (attributeType == UpgradeAttribute.ProjectileSize)
            {
                defaultValue = 1;
            }

            _upgradeAttributes.Add(attributeType, defaultValue);
        }

        return _upgradeAttributes[attributeType];
    }

    public void SetAttribute(UpgradeAttribute attributeType, float newValue)
    {
        if (!_upgradeAttributes.ContainsKey(attributeType))
        {
            float defaultValue = 0;
            if (attributeType == UpgradeAttribute.ProjectileSize)
            {
                defaultValue = 1;
            }

            _upgradeAttributes.Add(attributeType, defaultValue);
        }

        _upgradeAttributes[attributeType] = newValue;

        OnAttributeChanged?.Invoke(attributeType, newValue);
    }

    public void IncrementAttribute(UpgradeAttribute attributeType, float amountToIncrement)
    {
        SetAttribute(attributeType, GetAttribute(attributeType) + amountToIncrement);
    }

    public void DecrementAttribute(UpgradeAttribute attributeType, float amountToDecrement)
    {
        SetAttribute(attributeType, GetAttribute(attributeType) - amountToDecrement);
    }
}
