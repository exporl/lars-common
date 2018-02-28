using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.Xml;

public class BonusLayoutController : XmlLayoutController
{
    int current = 3;
    int total = 5;

    XmlElement iconContainer;

    //Action standardClickCallback, bonusClickCallback;

    public override void LayoutRebuilt(ParseXmlResult parseResult)
    {
        if (parseResult != ParseXmlResult.Changed) return;
        
        //drawBonusIcons();
    }

    public void setTotal(int n)
    {
        current = 0;
        total = n;
        drawBonusIcons();
    }

    public void setCurrent(int n)
    {
        current = n;
        drawBonusIcons();
    }

    /*
    public void setStandardCallback(Action callback)
    {
        standardClickCallback = callback;
    }

    public void setBonusCallback(Action callback)
    {
        bonusClickCallback = callback;
    }

    public void doBonusClick()
    {
        Debug.Log("Calling dobonusclick");
        if (bonusClickCallback != null)
            bonusClickCallback();
    }

    public void doStandardClick()
    {
        if (standardClickCallback != null)
            standardClickCallback();
    }
    */

    void drawBonusIcons()
    {
        // get the menu button container
        iconContainer = xmlLayout.GetElementById("iconContainer");
        
        // count children
        if (iconContainer.transform.childCount > 0)
        {
            for (var i = iconContainer.transform.childCount - 1; i >= 0; i--)
            {
                var child = iconContainer.transform.GetChild(i);
                child.transform.parent = null;
                //var xmlElement = child.GetComponent<XmlElement>();
                //xmlElement.enabled = false;
                Destroy(child.gameObject);
            }
        }

        // get the menu button template so that we can clone it
        var bonusIconTemplateOn = xmlLayout.GetElementById("bonusIconTemplateOn");
        var bonusIconTemplateOff = xmlLayout.GetElementById("bonusIconTemplateOff");

        // in this case, 'Examples' is populated in the editor
        for (int i = 0; i < total; i++)
        {
            var bonusIconTemplate = (i < current) ? bonusIconTemplateOn : bonusIconTemplateOff;

            // Create a copy of the template
            var bonusIcon = Instantiate(bonusIconTemplate);
            bonusIcon.name = (i > total - current) ? "off" : "on";

            // Access the XmlElement component and initialise it for this new button
            var xmlElement = bonusIcon.GetComponent<XmlElement>();
            xmlElement.Initialise(xmlLayout, (RectTransform)bonusIcon.transform, bonusIconTemplate.tagHandler);

            // Add the xmlElement to the menuButtonGroup
            iconContainer.AddChildElement(bonusIcon);

            // Set the necessary attributes, and Apply them
            //xmlSetAttribute("text", name);
            // the template is inactive (so as not to be visible), so we need to activate this button
            xmlElement.SetAttribute("active", "true");
            // Call the SelectExample function (in this XmlEventReceiver) when this button is clicked
            //xmlElement.SetAttribute("onClick", "hey();");
            xmlElement.ApplyAttributes();
        }
    }
}

