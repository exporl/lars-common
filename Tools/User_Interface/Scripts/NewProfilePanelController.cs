using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.Xml;
using UnityEngine.UI;

class NewProfilePanelController : XmlLayoutController
{
    public GameSettings settings;

    public override void LayoutRebuilt(ParseXmlResult parseResult)
    {
        // ParseXmlResult.Changed   => The Xml was parsed and the layout changed as a result
        // ParseXmlResult.Unchanged => The Xml was unchanged, so no the layout remained unchanged
        // ParseXmlResult.Failed    => The Xml failed validation

        // Called whenever the XmlLayout finishes rebuilding the layout
        // Use this function to make any dynamic changes (e.g. create dynamic lists, menus, etc.) or dynamically load values/selections for elements such as DropDown
    }

    void addProfileBt()
    {
        //create new data object & add it to profiles
        string nm = xmlLayout.GetElementById<InputField>("profileNameInput").text;
        settings.AddNewProfile(nm);

        //hide add panel
        gameObject.SetActive(false);
    }

    void cancelAddProfileBt()
    {
        //hide add panel
        gameObject.SetActive(false);
    }


}