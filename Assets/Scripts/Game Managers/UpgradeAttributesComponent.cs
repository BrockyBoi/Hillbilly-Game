using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weaponry;

class UpgradeAttributeAmounts
{
    public float FlatAmount;
    public float MultiplierAmount;

    public float GetTotalAmount()
    {
        return FlatAmount * MultiplierAmount;
    }
}

public class UpgradeAttributesComponent
{
    private Dictionary<UpgradeAttribute, UpgradeAttributeAmounts> _upgradeAttributes = new Dictionary<UpgradeAttribute, UpgradeAttributeAmounts>();

    public delegate void EOnAttributeChange(UpgradeAttribute attribute, float value);
    public event EOnAttributeChange OnAttributeChanged;

    public float GetModifiedAttributeValue(UpgradeAttribute attribute, float amountToModify, bool shouldIncrement = true)
    {
        CheckAttributeValidity(attribute);

        UpgradeAttributeAmounts attributeAmounts = _upgradeAttributes[attribute];
        float modifiedAttributeValue = 0;
        if (shouldIncrement)
        {
            modifiedAttributeValue = (amountToModify + attributeAmounts.FlatAmount) * attributeAmounts.MultiplierAmount;
        }
        else
        {
            modifiedAttributeValue = (amountToModify -  attributeAmounts.FlatAmount) * attributeAmounts.MultiplierAmount;
        }

        return modifiedAttributeValue;
    }

    public float GetFullAttributeValue(UpgradeAttribute attributeType)
    {
        CheckAttributeValidity(attributeType);

        return _upgradeAttributes[attributeType].GetTotalAmount();
    }

    public float GetFlatAttributeValue(UpgradeAttribute attributeType)
    {
        CheckAttributeValidity(attributeType);

        return _upgradeAttributes[attributeType].FlatAmount;
    }

    public float GetMultiplierAttributeValue(UpgradeAttribute attributeType)
    {
        CheckAttributeValidity(attributeType);

        return _upgradeAttributes[attributeType].MultiplierAmount;
    }

    public void SetAttributeAmount(UpgradeAttribute attributeType, bool isFlatValue, float newValue)
    {
        CheckAttributeValidity(attributeType);

        if (isFlatValue)
        {
            _upgradeAttributes[attributeType].FlatAmount = newValue;
        }
        else
        {
            _upgradeAttributes[attributeType].MultiplierAmount = newValue;
        }

        OnAttributeChanged?.Invoke(attributeType, _upgradeAttributes[attributeType].GetTotalAmount());
    }

    private void CheckAttributeValidity(UpgradeAttribute attributeType)
    {
        if (!_upgradeAttributes.ContainsKey(attributeType))
        {
            float defaultValue = attributeType == UpgradeAttribute.ProjectileSize ? 1 :0;
            UpgradeAttributeAmounts amounts = new UpgradeAttributeAmounts()
            {
                FlatAmount = defaultValue,
                MultiplierAmount = 1
            };

            _upgradeAttributes.Add(attributeType, amounts);
        }
    }

    public void IncrementAttributeAmount(UpgradeAttribute attributeType, bool isFlatValue, float amountToIncrement)
    {
        float newValue = (isFlatValue ? GetFlatAttributeValue(attributeType) : GetMultiplierAttributeValue(attributeType)) + amountToIncrement;
        SetAttributeAmount(attributeType, isFlatValue, newValue);
    }

    public void DecrementAttributeAmount(UpgradeAttribute attributeType, bool isFlatValue, float amountToDecrement)
    {
        float newValue = (isFlatValue ? GetFlatAttributeValue(attributeType) : GetMultiplierAttributeValue(attributeType)) - amountToDecrement;
        SetAttributeAmount(attributeType, isFlatValue, newValue);
    }
}
