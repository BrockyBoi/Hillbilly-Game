using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weaponry;
using XP;

class ScalableAttributeValue
{
    public float FlatAmount;
    public float MultiplierAmount;

    public float GetTotalAmount()
    {
        return FlatAmount * MultiplierAmount;
    }
}

public class CharacterAttributesManager
{
    private Dictionary<UpgradeAttribute, ScalableAttributeValue> _characterAttributes = new Dictionary<UpgradeAttribute, ScalableAttributeValue>();

    public delegate void EOnAttributeChange(UpgradeAttribute attribute, float value);
    public event EOnAttributeChange OnAttributeChanged;

    public float GetModifiedAttributeValue(UpgradeAttribute attribute, float amountToModify, bool shouldIncrement = true)
    {
        CheckAttributeValidity(attribute);

        ScalableAttributeValue attributeAmounts = _characterAttributes[attribute];
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

        return _characterAttributes[attributeType].GetTotalAmount();
    }

    public float GetFlatAttributeValue(UpgradeAttribute attributeType)
    {
        CheckAttributeValidity(attributeType);

        return _characterAttributes[attributeType].FlatAmount;
    }

    public float GetMultiplierAttributeValue(UpgradeAttribute attributeType)
    {
        CheckAttributeValidity(attributeType);

        return _characterAttributes[attributeType].MultiplierAmount;
    }

    public void SetAttributeAmount(UpgradeAttribute attributeType, bool isFlatValue, float newValue)
    {
        CheckAttributeValidity(attributeType);

        if (isFlatValue)
        {
            _characterAttributes[attributeType].FlatAmount = newValue;
        }
        else
        {
            _characterAttributes[attributeType].MultiplierAmount = newValue;
        }

        OnAttributeChanged?.Invoke(attributeType, _characterAttributes[attributeType].GetTotalAmount());
    }

    private void CheckAttributeValidity(UpgradeAttribute attributeType)
    {
        if (!_characterAttributes.ContainsKey(attributeType))
        {
            ScalableAttributeValue amounts = new ScalableAttributeValue()
            {
                FlatAmount = 0,
                MultiplierAmount = 1
            };

            _characterAttributes.Add(attributeType, amounts);
        }
    }

    public void AddTrait(CharacterTrait trait)
    {
        foreach (CharacterAttributeModifier attribute in trait.CharacterAttributes)
        {
            IncrementAttributeAmount(attribute);
        }
    }

    public void IncrementAttributeAmount(CharacterAttributeModifier attributeModifier)
    {
        IncrementAttributeAmount(attributeModifier.AttributeType, attributeModifier.IsFlatAmount, attributeModifier.AttributeAmount);
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
